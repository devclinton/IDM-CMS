import os, sys
import numpy.random as rndgen
from scipy.stats import johnsonsu
import pylab as pyl

sys.path.insert(0, '../SSA') # include SSA directory in search path
from SSAOpenCL import *

def GetN():
    return GetN.NRuns
GetN.NRuns = 1000


def probRound(vals):
	randvals = np.random.rand(len(vals))
	remainder = np.mod(vals,1.0)
	vals = vals.copy()
	vals[remainder>randvals] = np.ceil(vals[remainder>randvals])
	vals[remainder<=randvals] = np.floor(vals[remainder<=randvals])
	vals[vals<0] = 0.0
	return vals



#### ---------------------- Macroscales ------------------------

class macroBase(object):


    def __init__(self, time, state, perad):
        self.time = time
        self.state = state
        self.perad = perad
        self.NRuns = GetN()

    def copy(self):
        return type(self)(self.time, self.state.copy(), self.perad)

    def __add__(self, other):
        if isinstance(other, float) or isinstance(other, int):
            return type(self)(self.time+other, self.state+other, self.perad+other)
        else:
            return type(self)(self.time+other.time, self.state+other.state, self.perad+other.perad)

    def __sub__(self, other):
        if isinstance(other, float) or isinstance(other, int):
            return type(self)(self.time-other, self.state-other,self.perad-other)
        else:
            return type(self)(self.time-other.time, self.state-other.state,self.perad-other.perad)

    def __mul__(self, other):
        if isinstance(other, float) or isinstance(other, int):
            return type(self)(self.time*other, self.state*other, self.perad*other)
        else:
            assert "ERROR, Not Implemented"

    def __div__(self,other):
            return type(self)(self.time/other, self.state/other, self.perad/other)

    def __rmul__(self, other):
        if isinstance(other, float) or isinstance(other, int):
            return type(self)(self.time*other, self.state*other, self.perad*other)


class microBase(object):

    def __init__(self, time,state,perad=0):
        self.time = time
        self.state = np.r_[state.reshape(-1)]
        self.perad = perad
        self.NRuns = GetN()

# ------------------------ Mean ---------------------------

class macroMean(macroBase):

    def __init__(self, time, state, perad=0.0):
        super(macroMean,self).__init__(time,state,perad)

    def lift(self):
        return microMean(self.time, probRound(np.tile(self.state, self.NRuns).reshape(-1)), self.perad)

    def stats(self):
        NSpecies = len(self.state)
        output = np.zeros(NSpecies*2+2, "float")
        output[0] = self.time
        output[1:NSpecies+1] = self.state
        output[-1] = self.perad

        return output

class microMean(microBase):
    def __init__(self, time, state, perad=0.0):
        super(microMean,self).__init__(time,state, perad)

    def restrict(self):

        Wild = np.sum(self.state.reshape(-1,10)[:,1:5],axis=1)
        Nonzero = np.sum(Wild>0)
        NTot = self.NRuns/(1-self.perad)
        return macroMean(self.time, np.mean(self.state.reshape(-1,10)[Wild>0,:], axis=0).reshape(-1), self.perad+(self.NRuns-Nonzero)/NTot)




# ------------------------ MeanStd ---------------------------

class macroMeanStd(macroBase):
    def __init__(self, time, state, perad=0.0):
        super(macroMeanStd,self).__init__(time,state,perad)

    def lift(self):
        NStates = len(self.state)/2
        Means = self.state[:NStates]
        Stds = self.state[NStates:]
        randvec = rndgen.randn(NStates*self.NRuns)*np.tile(Stds,self.NRuns) + np.tile(Means, self.NRuns)

        return microMeanStd(self.time, probRound(randvec), self.perad)

    def stats(self):
        NSpecies = len(self.state)/2
        output = np.zeros(NSpecies*2+2, "float")
        output[0] = self.time
        output[1:-1] = self.state
        output[-1] = self.perad

        return output

class microMeanStd(microBase):
    def __init__(self, time, state, perad=0.0):
        super(microMeanStd,self).__init__(time,state, perad)

    def restrict(self):
        Wild = np.sum(self.state.reshape(-1,10)[:,1:5],axis=1)
        Nonzero = np.sum(Wild>0)
        NTot = self.NRuns/(1-self.perad)
        means = np.mean(self.state.reshape(-1,10)[Wild>0,:], axis=0)
        stds = np.std(self.state.reshape(-1,10)[Wild>0,:], axis=0)
        return macroMeanStd(self.time, np.r_[means.reshape(-1), stds.reshape(-1)], self.perad+float(self.NRuns-Nonzero)/NTot)

    def stats(self):
        NSpecies = len(self.state)/2
        output = np.zeros(NSpecies*2+2, "float")
        output[0] = self.time
        output[1:NSpecies+1] = self.state[:NSpecies]
        output[NSpecies+1:-1] = self.state[NSpecies:]
        output[-1] = self.perad

        return output

# ------------------------ MeanCov---------------------------

class macroMeanCov(macroBase):
    def __init__(self, time, state, perad=0.0):
        super(macroMeanCov,self).__init__(time,state,perad)

    def lift(self):
        NStates = int(-0.5+0.5*np.sqrt(1+4*len(self.state)))
        Means = self.state[:NStates]
        Cov = self.state[NStates:].reshape(NStates,NStates)
        Vals,Vecs = np.linalg.eigh(Cov)
	Vals[Vals<0] = 0.0
        randvals = np.random.randn(NStates,self.NRuns)
        states = np.dot(Vecs, np.dot(np.diag(np.sqrt(Vals)),randvals)).T
        randvec = states.reshape(-1) + np.tile(Means, self.NRuns)

        return microMeanCov(self.time, probRound(randvec), self.perad)

    def stats(self):
        NSpecies =  int(-0.5+0.5*np.sqrt(1+4*len(self.state)))
        output = np.zeros(NSpecies*2+2, "float")
        output[0] = self.time
        output[1:NSpecies+1] = self.state[:NSpecies]
        output[NSpecies+1:-1] = np.sqrt(self.state[NSpecies::NSpecies])
        output[-1] = self.perad

        return output
    
class microMeanCov(microBase):
    def __init__(self, time, state, perad=0.0):
        super(microMeanCov,self).__init__(time,state, perad)

    def restrict(self):
        Wild = np.sum(self.state.reshape(-1,10)[:,1:5],axis=1)
        Nonzero = np.sum(Wild>0)
        NTot = self.NRuns/(1-self.perad)
        means = np.mean(self.state.reshape(-1,10)[Wild>0,:], axis=0)
        cov = np.cov(self.state.reshape(-1,10)[Wild>0,:].T)
        return macroMeanCov(self.time, np.r_[means.reshape(-1), cov.reshape(-1)], self.perad+float(self.NRuns-Nonzero)/NTot)


# ------------------------ MeanPCA---------------------------

class macroMeanPCA(macroBase):
    def __init__(self, time, state, perad=0.0):
        super(macroMeanPCA,self).__init__(time,state,perad)

    def lift(self):
        NStates = int(-1+np.sqrt(1.0+len(self.state)))
        Means = self.state[:NStates]
	Umat = self.state[NStates:NStates+NStates**2].reshape(NStates,NStates)
	StdMat = self.state[NStates+NStates**2:]

	RandMat = np.dot(Umat, np.dot(np.diag(StdMat),np.random.randn(NStates,self.NRuns))).T.reshape(-1)
        randvec = RandMat + np.tile(Means, self.NRuns)

        return microMeanPCA(self.time, probRound(randvec), self.perad)

    def stats(self):
        NStates = int(-1+np.sqrt(1.0+len(self.state)))
	data = self.lift().state.reshape(-1,NSpecies)
        means = np.mean(data, axis=0)
        stds = np.std(data,axis=0)	
	
        output = np.zeros(NStates*2+2, "float")
        output[0] = self.time
        output[1:NSpecies+1] = means
        output[NSpecies+1:-1] = stds
        output[-1] = self.perad

        return output
    
class microMeanPCA(microBase):
    def __init__(self, time, state, perad=0.0):
        super(microMeanPCA,self).__init__(time,state, perad)

    def restrict(self):
        Wild = np.sum(self.state.reshape(-1,10)[:,1:5],axis=1)
        Nonzero = np.sum(Wild>0)
        NTot = self.NRuns/(1-self.perad)
	
	# Compute covariances
	data = self.state.reshape(-1,10)[Wild>0,:].T
	means = np.mean(data, axis=1)
	data = (data.T.reshape(-1)-np.tile(means,Nonzero)).reshape(-1,10)
	
	try:
	    pcanode = mdp.nodes.JADENode()
	    pcanode.train(data)
	    pcanode.stop_training()
	
	    randvals = pcanode.execute(data)
	    mat = get_projmatrix()
	except:
	    NSpecies = data.shape[1]
	    randvals = np.zeros(data.shape)
	    mat = np.zeros((NSpecies, NSpecies))
	
	randvals[np.isnan(randvals)] = 0.0
	mat[np.isnan(mat)] = 0.0
	print np.mean(randvals,axis=0), np.std(randvals,axis=0)
	
        return macroMeanPCA(self.time, np.r_[means.reshape(-1), mat.reshape(-1), np.std(randvals,axis=0)], self.perad+float(self.NRuns-Nonzero)/NTot)



# ------------------------ MeanJohnson---------------------------

class macroMeanJohnson(macroBase):
    def __init__(self, time, state, perad=0.0):
        super(macroMeanJohnson,self).__init__(time,state,perad)

    def lift(self):
        NStates = int(-2.5+0.5*np.sqrt(25+4*len(self.state)))
        Means = self.state[:NStates]
        Cov = self.state[NStates:NStates+NStates**2].reshape(NStates,NStates)
	Params = self.state[NStates+NStates**2:]
	vals,vecs = np.linalg.eigh(Cov)
	vals[vals<0] = 0.0
	
	randvals = np.zeros((NStates, self.NRuns))
	for ii in xrange(0, NStates):
	    randvals[ii,:] = johnsonsu.rvs(Params[4*ii], Params[4*ii+1], loc=Params[4*ii+2], scale=Params[4*ii+3], size=self.NRuns)
        states = np.dot(vecs, randvals).T
	
        randvec = states.reshape(-1) + np.tile(Means, self.NRuns)

        return microMeanJohnson(self.time, probRound(randvec), self.perad)

    def stats(self):
	
        NSpecies =  int(-0.5+0.5*np.sqrt(1+4*len(self.state)))
	data = self.lift().state.reshape(-1,NSpecies)
        means = np.mean(data, axis=0)
        stds = np.std(data,axis=0)	
	
        output = np.zeros(NSpecies*2+2, "float")
        output[0] = self.time
        output[1:NSpecies+1] = means
        output[NSpecies+1:-1] = stds
        output[-1] = self.perad

        return output
    
class microMeanJohnson(microBase):
    def __init__(self, time, state, perad=0.0):
        super(microMeanJohnson,self).__init__(time,state, perad)

    def restrict(self):
        Wild = np.sum(self.state.reshape(-1,10)[:,1:5],axis=1)
        Nonzero = np.sum(Wild>0)
        NTot = self.NRuns/(1-self.perad)
	
	# Compute covariances
	data = self.state.reshape(-1,10)[Wild>0,:].T
	means = np.mean(data, axis=1)
	data = (data.T.reshape(-1)-np.tile(means,Nonzero)).reshape(-1,10).T
	Cov = np.cov(data)
	vals,vecs = np.linalg.eigh(Cov)

	modeAmplitude = np.dot(vecs.T, data)
	
	shape = np.zeros(4*modeAmplitude.shape[0])
	for ii in xrange(0, modeAmplitude.shape[0]):
	    shape[4*ii:4*ii+4] = johnsonsu.fit(modeAmplitude[ii,:],0.9,0.9)
	
        return macroMeanJohnson(self.time, np.r_[means.reshape(-1), Cov.reshape(-1), shape.reshape(-1)], self.perad+float(self.NRuns-Nonzero)/NTot)


# ---------------------------------------------------------


class SSAWrapper(SSA):
    def __init__(self, dt, emodl, cfgFile,path=''):
        super(SSAWrapper, self).__init__(emodl, cfgFile,path,GetN())
        self.dt = dt

    def microSim(self,micro):

        NIterations = int(np.ceil(float(micro.NRuns)/self.NWorkers))
        self.targetTau = self.dt+micro.time
        self.tau.fill(micro.time)
        self.Species.set(micro.state.astype(np.float32))
        interrupt = 1

        # Initialize Observables
        for ii in xrange(0, NIterations):
	    offset = self.NWorkers*ii
            event = self.SSAprgStep.run(self.queue, (self.NWorkers,), None, self.tau.data,
                                   self.Species.data, self.Parameters.data,
                                   self.Observables.data, self.randGen,
                                   np.float32(self.targetTau), np.int32(offset), np.int32(micro.NRuns))
	    event.wait()
        return type(micro)(micro.time+self.dt, self.Species.get().astype(np.float), micro.perad)




