from EQFWrapper import *

## ------------------- Time Steppers SSA ------------------------------------
class EulerStep(SSAWrapper):
    def __init__(self, dt, K, M, emodl, cfgFile, path=''):
        super(EulerStep,self).__init__( dt, emodl, cfgFile,path)
        self.K = K
        self.M = M

    def Project(self, macro):

        self.microlist = []
        microval = macro.lift() # compute initial condition
        for ii in xrange(0, self.K):
            microval = self.microSim(microval)

        self.microlist.append(microval)
        macrolist = [microval.restrict()]
        microval = self.microSim(microval)
        self.microlist.append(microval)
        macrolist.append(microval.restrict())
        nu = (macrolist[1]-macrolist[0])/self.dt

        Old = None

        for val in macrolist:
            print val.time, val.perad

            if Old != None:
                if(Old>val.perad):
                    asdfasdfasfsaf
            Old = val.perad

        return macrolist[1] + self.M*self.dt*nu

class RK2Step(SSAWrapper):
    def __init__(self, dt, K, M, emodl, cfgFile, path=''):
        super(RK2Step,self).__init__(dt, emodl, cfgFile,path)
        self.K = K
        self.M = M

    def Project(self,macro):
        # ---------- Euler step ----------------
        microval = macro.lift() # compute initial condition
        for ii in xrange(0, self.K):
            microval = self.microSim(microval)

        macrolist = [microval.restrict()]
        macrolist.append(self.microSim(microval).restrict())
        nu = (macrolist[1]-macrolist[0])/self.dt
        macroTilde = macrolist[1] + self.M*self.dt*nu
        # -----------
        microval = macroTilde.lift() # compute initial condition
        for ii in xrange(0, self.K):
            microval = self.microSim(microval)

        macrolist2 = [microval.restrict()]
        macrolist2.append(self.microSim(microval).restrict())
        nu2 = (macrolist2[1]-macrolist2[0])/self.dt

        alpha = float(self.M+1+2*self.K)/(2*(self.M+1+self.K))
        return macrolist[1] + self.M*self.dt*(alpha*nu+(1-alpha)*nu2)


class PABStep(SSAWrapper):
    def __init__(self, dt, K, M, emodl, cfgFile, path=''):
        super(PABStep,self).__init__(dt, emodl, cfgFile,path)
        self.K = K
        self.M = M
        self.OldNu = None

    def Initialize(self):
        self.OldNu = None

    def Project(self,macro):
        # ---------- Euler step ----------------
        microval = macro.lift() # compute initial condition
        for ii in xrange(0, self.K):
            microval = self.microSim(microval)

        macrolist = [microval.restrict()]
        macrolist.append(self.microSim(microval).restrict())
        nu = (macrolist[1]-macrolist[0])/self.dt

        if self.OldNu == None: # Take Euler step for first step
            self.OldNu = nu.copy()
            return macrolist[1] + self.M*self.dt*nu
        else:
            alpha = 1.0 + 0.5*(self.M+1)/(self.M+1+self.K)
            Output = macrolist[1] + self.M*self.dt*(alpha*nu +(1-alpha)*self.OldNu)
            self.OldNu = nu.copy()
            return Output

class PABAdaptStep(SSAWrapper):
    def __init__(self, dt, K, M, emodl, cfgFile, path=''):
        super(PABAdaptStep,self).__init__(dt, emodl, cfgFile,path)
        self.K = K
        self.M = 0.0
        self.epsilon = 1e-2
        self.OldNu = None
        self.OldM = 0.0

    def Initialize(self):
        self.OldNu = None
        self.OldM = 0.0
        self.M = 0.0

    def Project(self,macro):
        # ---------- Euler step ----------------
        microval = macro.lift() # compute initial condition
        for ii in xrange(0, self.K):
            microval = self.microSim(microval)

        macrolist = [microval.restrict()]
        macrolist.append(self.microSim(microval).restrict())
        nu = (macrolist[1]-macrolist[0])/self.dt

        if self.OldNu == None: # Take Euler step for first step with no projection
            self.OldNu = nu.copy()
            self.OldM = self.M
            return macrolist[1] + self.M*self.dt*nu
        else:
            weight = macrolist[1].state.copy()
            weight[weight>1.0] = 1/np.sqrt(weight[weight>1.0])

            rhs = 2*(self.OldM+self.K+1.0)*self.epsilon*np.sqrt(np.sum(abs(macrolist[1].state*weight)**2))/(self.dt*np.sqrt(np.sum(abs((nu.state-self.OldNu.state)*weight)**2)))
            self.M = 0.5*np.sqrt(1.0+4*rhs)-0.5
            if self.M>2.0:
                self.M = 2.0
            alpha = 1.0 + 0.5*(self.M+1)/(self.OldM+1+self.K)
            Output = macrolist[1] + self.M*self.dt*(alpha*nu +(1-alpha)*self.OldNu)

            self.OldNu = nu.copy()
            self.OldM = self.M

            return Output
