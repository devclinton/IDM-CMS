import numpy as np
import pyopencl as cl
import pyopencl.array as cl_array
from emodlParse import *
import time

class SSA(object):
    """
    This class sets up the OpenCL context and queue and handles 
    the communication between the GPU (which computes individual SSA runs)
    and the CPU (where all other work is done).  
    """
    
    def __init__(self, emodl, cfgFile, path='', runs=None):
        """
        Constructor for the SSA class.  Takes in and parses an emodl file and config file.
        """

        self.path = path # path to the directory where model and config files lay
        self.ctx = cl.create_some_context() # create an OpenCL context
        self.queue = cl.CommandQueue(self.ctx) # create an OpenCL command queue in this context
        self.parser = EMODLParser(emodl, "SSA", path) # parse the emodl file
        self.parser.generate(self.queue) # compute parameter value, model parameters, etc


        ## Read configuration json file
        cfg = json.load(open(path+cfgFile))
        if runs == None:
            self.runs = np.int32(cfg["runs"])
        else:
            self.runs = np.int32(runs)
            
        self.duration = cfg["duration"] #extract duration 
        self.samples = cfg["samples"] # extact samples

        # Compute binaries 
        self.progArray = {} # empty dictionary for binary memoization 
        self.compileModel()
        
        # We stop either at event times, when time-events occur, or when outputs are requested 
        recordInd = 0 # Number of stops to record data
        eventInd = 0 # Number of time events 

        RecordStops = np.linspace(0, self.duration,self.samples) # linearly distribute 
        EventStops = self.parser.Events # copy list of time events from the parser
        
        # Combine the two lists to produce a single list with all the stops
        self.stopVec = []
        while recordInd < self.samples:
            if (len(EventStops) == 0) or not (len(EventStops)>eventInd):
                self.stopVec.append([RecordStops[recordInd], "record"])
                recordInd += 1
            elif EventStops[eventInd] > RecordStops[recordInd]:
                self.stopVec.append([RecordStops[recordInd], "record"])
                recordInd += 1
            else:
                self.stopVec.append([EventStops[eventInd],"event"])
                eventInd += 1
        
        # Determine number of threads per block (multiple threads for GPU and single for CPU)
        self.NWorkers = self.parser.NLocal*self.queue.device.max_compute_units
        
        # Allocate Device Memory
        self.randGen = cl.Buffer(self.ctx, cl.mem_flags.READ_WRITE, size=112*self.NWorkers)
        self.tau = cl_array.to_device(self.ctx, self.queue, np.zeros(self.runs,"float32"))
        self.Species = cl_array.to_device(self.ctx, self.queue, np.zeros(self.runs*self.parser.NSpecies, "float32"))
        self.Observables = cl_array.to_device(self.ctx, self.queue, np.zeros(self.runs*self.parser.NObservables, "float32"))
        self.Reactions = cl_array.to_device(self.ctx, self.queue, np.zeros(self.parser.NReactions*self.NWorkers))
        
        # Initialize random number generator
        seed = int(time.time()*1e6) % 2<<30
        self.RNGprg.init_ranlux(self.queue, (self.NWorkers,), None,
                                np.uint32(seed), self.randGen,
                                np.uint32(self.NWorkers))
                                
        # Set initial condition based off the .emodl file (this may be wasted effort if the emodl file doesn't include an IC)
        self.setIC(np.tile(np.array(self.parser.IC),self.runs))

    def compileModel(self):
        """
        This function generates the OpenCL binary from program text.  To save on time, 
        we memoize and store the binary of any file that has previously been compiled. 
        
        NOTE: In systems with many different time-events, say SIAs, this function can 
        consume a large fraction of the computational time. 
        """
        t1 = time.time()
        Paramstr = self.parser.generateParamString()

        if Paramstr in self.progArray:
            print "Loading Kernel..."
            self.RNGprg,self.SSAprgInit,self.SSAprgStep,self.SSAprgEvent = self.progArray[Paramstr]
        else:
            print "Compiling Kernel..."
            self.RNGprg = cl.Program(self.ctx,self.parser.replaceParams(randString)).build()  # code for random number generator
            self.SSAprgInit = cl.Program(self.ctx, self.parser.replaceParams(self.parser.fileStringInit)).build()  # code for SSA initialization
            self.SSAprgStep = cl.Program(self.ctx, self.parser.replaceParams(self.parser.fileStringStep)).build()  # code for SSA run
            self.SSAprgEvent = cl.Program(self.ctx, self.parser.replaceParams(self.parser.fileStringEvent)).build()  # code for SSA run with a time-event
            
            # Memoize binaries 
            self.progArray[Paramstr] = (self.RNGprg,self.SSAprgInit,self.SSAprgStep,self.SSAprgEvent)
            
        print "Model built in", time.time()-t1, "seconds"

        

    def setIC(self, IC):
        """
        Sent the initial condition array in IC to the GPU
        """
        
        if(IC.shape != self.Species.shape):
            raise Exception("Initial condition size mismatch")
        self.Species = cl_array.to_device(self.ctx, self.queue, IC.astype("float32"))

    def step(self,offset,interrupt):
        """
        Step the state of the system forward in time.  Either keep the last step (if all we want 
        are the observables) or reject it (if we apply a time changes).
        """
        
        if interrupt == 0:
            event = self.SSAprgStep.run(self.queue, (self.NWorkers,), None, self.tau.data,
                                   self.Species.data, self.Reactions.data,
                                   self.Observables.data, self.randGen,
                                   np.float32(self.targetTau), np.int32(offset), self.runs)
        else:
            event = self.SSAprgEvent.run(self.queue, (self.NWorkers,), None, self.tau.data,
                                   self.Species.data, self.Reactions.data,
                                   self.Observables.data, self.randGen,
                                   np.float32(self.targetTau), np.int32(offset), self.runs)
        event.wait() # wait for event to terminate


    def solve(self,outputname="trajectories.dat"):
        """
        Evolve the state of the system forward in time, and return the observable state at the
        times requested in the emodl file.
        """
        
        NIterations = int(np.ceil(float(self.runs)/self.NWorkers)) # number of GPU iterations needed 
                                                                   # to cover the number of runs 
                                                                   
                    
        # Allocate output memory
        output = np.zeros((self.parser.NObservables*self.runs, self.samples), "float32")


        # Initialize Observables
        for ii in xrange(0, NIterations):
            self.SSAprgInit.run(self.queue, (self.NWorkers,), None, self.tau.data,
                                   self.Species.data, self.Reactions.data,
                                   self.Observables.data, self.randGen,
                                   np.float32(0.0), np.int32(self.NWorkers*ii), self.runs).wait()
            
        tcurr = 0.0
        recordIter = 0; eventIter = 0
        rebuild = False

        
        for timeval in self.stopVec:
            self.targetTau = np.float32(timeval[0])
            if timeval[1] == "record":
                interrupt = 0
            else:
                interrupt = 1
            
            if np.abs(tcurr-timeval[0])>1e-10: #step if next event is in the future
                if rebuild == True:
                    self.compileModel()
                    rebuild = False
                    
                t1 = time.time()
                for ii in xrange(0, NIterations):
                    self.step(self.NWorkers*ii,interrupt)
                print "Computation done in ", time.time() - t1, "seconds"
                tcurr = timeval[0]
                
            if timeval[1] == "record":
                output[:,recordIter] = (self.Observables.get().reshape(-1,self.parser.NObservables)).T.reshape(-1)
                recordIter+=1
                
            elif timeval[1] == "event": # update events
                rebuild = True
                self.parser.updateParamVec(self.parser.Changes[eventIter])
                eventIter += 1

        np.savetxt(outputname,output)

