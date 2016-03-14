from EQFTimesteppers import *

## ----------------- EQF Wrapper ----------------------------
class EQF(object):
    def __init__(self, stepper, IC, t0,tf):
        self.stepper = stepper # timestepper.  Function "Project" projects forward in time
        self.t0 = t0
        self.t = t0
        self.tf = tf
        self.IC = IC # macroscale with desired dynamics

        self.Output = [IC]

    def CPI(self):

        for ii in xrange(0, 10**6):
            print "T = ", self.Output[-1].time
            #dadffasdf
            self.Output.append(self.stepper.Project(self.Output[-1]))
            
            if self.Output[-1].time > self.tf:
                break
        return self.Output

