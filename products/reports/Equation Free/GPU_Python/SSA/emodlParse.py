import re
import json
from string import whitespace
from mako.template import Template
import numpy as np
from collections import OrderedDict
from SSATemplates import *
import pyopencl as cl


## Parse Prefix notation
atom_end = set('()') | set(whitespace)

def parse(sexp):
    stack, i, length = [[]], 0, len(sexp)
    while i < length:
        c = sexp[i]
        reading = type(stack[-1])
        if reading == list:
            if   c == '(': stack.append([])
            elif c == ')':
                stack[-2].append(stack.pop())
            elif c in whitespace: pass
            else: stack.append((c,))
        elif reading == str:
            if   c == '"':
                stack[-2].append(stack.pop())
                if stack[-1][0] == ('quote',): stack[-2].append(stack.pop())
            elif c == '\\':
                i += 1
                stack[-1] += sexp[i]
            else: stack[-1] += c
        elif reading == tuple:
            if c in atom_end:
                atom = stack.pop()
                stack[-1].append(atom[0])
                continue
            else: stack[-1] = ((stack[-1][0] + c),)
        i += 1
    return stack.pop()



class EMODLParser(object):
    """
    This class reads an EMODL file and produces a valid string containing the OpenCL code needed 
    """

    commentchar = ';'


    def __init__(self, file, outputType="SSA",path=''):


        fid = open(path+file)
        self.outputType = outputType
        self.path = path
        
        # Extract lines from file and remove comments.  Then reform queue
        self.queue = []
        cmdstr = ""

        for line in fid:
            tmp = line.strip().split(self.commentchar,1)[0]
            if len(tmp) > 0:
                cmdstr += tmp

        # Generate a list of commands
        self.cmdList = parse(cmdstr)




    def generate(self,queue):
        """
        generate allocates memory and initializes values for 
        """
        # Count number of each type
        self.NSpecies = 0
        self.NObservables = 0
        self.NReactions = 0
        self.NParams = 0
        self.NFuncs = 0

        # Map Variable Names to Index
        self.Species = {}
        self.Observables = {}
        self.Reactions = {}
        self.Params = {}
        self.ParamValues = {}
        self.Events = []
        self.Changes = []
        self.Funcs = {}
        self.jsons = {}
        
        self.ParamString = ""

        self.IC = []
        self.Decrement = []
        self.Increment = []

        # Generate command lines and compute propensities
        cmdLines = []
        for cmd in self.cmdList:
            tmp = self.generateList(cmd)
            if tmp != None:
                cmdLines.append(tmp)
                

        # Generate string of reaction updates
        switchTemp = Template("""
        case ${ind}:
            ${react}
            break;
        """)
        incTemp = Template("Species[lrow+${ind}]${op}= 1.0;\n")


        switchString = ''
        for ii in xrange(0, self.NReactions):
            updateString = ''
            for spec in self.Decrement[ii]:
                updateString += incTemp.render(ind=self.Species[spec],op="-")
            for spec in self.Increment[ii]:
                updateString += incTemp.render(ind=self.Species[spec],op="+")
            switchString += switchTemp.render(ind=ii,react=updateString)


        # Compute number of work items this device can support
        if queue.device.type == cl.device_type.CPU:
            self.NLocal = int(1)
        else:
            self.NLocal = int(queue.device.local_mem_size/((self.NSpecies+self.NObservables+self.NFuncs)*8))

        if self.NLocal > queue.device.max_work_group_size:
            self.NLocal = queue.device.max_work_group_size

        self.fileStringInitTemplate = Template(SSAStringInit)
        self.fileStringInit = self.fileStringInitTemplate.render(NSPECIES=self.NSpecies,
                                NOBSERVATIONS=self.NObservables, 
                                NREACTIONS=self.NReactions,changeString=switchString,
                                cmdList=cmdLines,NFUNCS=self.NFuncs,NLOCAL=self.NLocal)

        self.fileStringStepTemplate = Template(SSARunStepString)
        self.fileStringStep = self.fileStringStepTemplate.render(NSPECIES=self.NSpecies,
                                NOBSERVATIONS=self.NObservables, 
                                NREACTIONS=self.NReactions,changeString=switchString,
                                cmdList=cmdLines,NFUNCS=self.NFuncs,NLOCAL=self.NLocal)
        
        self.fileStringEventTemplate = Template(SSARunStepString)
        self.fileStringEvent = self.fileStringEventTemplate.render(NSPECIES=self.NSpecies,
                                NOBSERVATIONS=self.NObservables, 
                                NREACTIONS=self.NReactions,changeString=switchString,
                                cmdList=cmdLines,NFUNCS=self.NFuncs,NLOCAL=self.NLocal)

                            
        # Sort events
        if len(self.Events) > 0:
            eventInd = np.argsort(np.array(self.Events))
            output = []
            
            for ii in xrange(0, len(eventInd)):
                output.append(self.Changes[eventInd[ii]])
            
            self.Events = np.array(np.array(self.Events)[eventInd])
            self.Changes = np.array(np.array(self.Changes)[eventInd])
            

    def generateList(self, cmd):
        """
        generateList parses the expression tree in cmd and loads the names of parameters, functions,
        species, observables, and reactions into the associated memory locations in the class
        """
        
        # Atomic expression parsing 
        if type(cmd) is not list:
            cmd = cmd.strip()
            if cmd in self.Species.keys():
                return Template("Species[${ind}]").render(ind=self.Species[cmd])
            elif cmd in self.Observables.keys():
                return Template("Obs[${ind}]").render(ind=self.Observables[cmd])
            elif cmd in self.Params.keys():
                return "Parameter{0:06d}".format(self.Params[cmd])
            elif cmd in self.Funcs.keys():
                return Template("Func[${ind}]").render(ind=self.Funcs[cmd])
            elif cmd == "time":
                return "time"
            elif cmd == "pi":
                return str(float(np.pi))+'f'
            else:
                return str(float(cmd))+'f'

        expr = cmd[0]
        Nargs = len(cmd) # compute number of args
        if expr in ["import", "emodl", "start-model", "end-model", "locale", "set-locale"]: # import is no-op
            return ''
            
        elif expr == "species": # define species
            name = cmd[1]
            self.Species[name] = self.NSpecies
            self.NSpecies+=1

            if len(cmd) > 2:
                self.IC.append(self.generateList(cmd[2]))
                if self.IC[-1][-1]=='f': # remove appended 'f' 
                    self.IC[-1]=self.IC[-1][:-1]
            else:
                self.IC.append(0.0)
                    
            return None


        elif expr == "param": # define parameters
            name = cmd[1]
            self.Params[name] = self.NParams
            self.NParams+=1
            self.ParamValues[name] = self.generateList(cmd[2])
            return None

        elif expr == "observe": # define observables
            name = cmd[1]
            self.Observables[name] = self.NObservables
            self.NObservables+=1
            return Template("Obs[${ind}]=${rhs};").render(ind=self.Observables[name], rhs=self.generateList(cmd[2]))

        elif expr == "func": # define observables
            name = cmd[1]
            self.Funcs[name] = self.NFuncs
            self.NFuncs+=1
            return Template("Func[${ind}]=${rhs};").render(ind=self.Funcs[name], rhs=self.generateList(cmd[2]))

        elif expr == "sum":
            expr = "+"
            if len(cmd) == 2: # unitary operator
                return expr + self.generateList(cmd[1])
            else:
                tmpstr = "("+self.generateList(cmd[1])
                for ii in xrange(2,len(cmd)):
                    tmpstr += expr + self.generateList(cmd[ii])
                return tmpstr+")"

        elif expr in ["+","-","*", "/"]: # mathematical expressions
            if len(cmd) == 2: # unitary operator
                return expr + self.generateList(cmd[1])
            else:
                tmpstr = "("+self.generateList(cmd[1])
                for ii in xrange(2,len(cmd)):
                    tmpstr += expr + self.generateList(cmd[ii])
                return tmpstr+")"

        elif expr in ['sin','cos','tan','exp']:
            return expr+"("+self.generateList(cmd[1])+")"

        elif expr == "reaction": # define observables
            name = cmd[1]
            self.Reactions[name] = self.NReactions

            self.Decrement.append(cmd[2])
            self.Increment.append(cmd[3])
            self.NReactions+=1
            
            if self.Reactions[name] == 0:
                append = ''
            else:
                append = "+ gReactions[greact+{0}]".format(self.Reactions[name]-1)
                
            return Template("gReactions[greact+${ind}]=${rhs} ${append};").render(rhs=self.generateList(cmd[4]),ind=self.Reactions[name],append=append)

        elif expr == "time-event": # Parse time event
            
            self.Events.append(float(eval(cmd[2]))) # same time the event occurs
            self.Changes.append(self.assignValue(cmd[3]))
            return None


        elif expr == "json":
            fid = open(self.path+eval(cmd[2]))
            self.jsons[cmd[1]] = json.load(fid)

        elif expr in self.jsons.keys():
            tmp = self.jsons[expr]
            levels = eval(cmd[1]).split('.')
            for lvl in levels:
                tmp = tmp[lvl]
            return str(float(tmp))+'f'
        else:
            raise Exception('Function {0} not implemented'.format(expr))

    def assignValue(self, cmdList):
        
        returnDict = {}
        for cmd in cmdList:
            returnDict[cmd[0]] = self.generateList(cmd[1])
        return returnDict
        
    def generateParamString(self):
        outstr = ""
        
        for key in self.Params.keys():
            outstr += "#define Parameter"+str(self.Params[key])+ " " + self.ParamValues[key] + "\n"
        return outstr

    def updateParamVec(self, newParams):
        self.ParamValues.update(newParams)

    def replaceParams(self, prgString):
        
        for key in self.Params.keys():
            prgString = prgString.replace("Parameter{0:06d}".format(self.Params[key]),self.ParamValues[key])

        return prgString