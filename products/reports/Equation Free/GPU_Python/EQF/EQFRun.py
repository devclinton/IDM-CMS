from EQF import *

GetN.NRuns = 100000
NRuns = GetN()
ICFull = np.tile(np.array([16680, 74, 74, 320, 320,0,0,0,0,200200],"float64"),NRuns)
microIC = microMeanJohnson(0.0, ICFull, 0.0).restrict()

#print microIC.state.shape
Stp = EulerStep(2./12, 1, 0, 'EDVaccinate.emodl', 'config.json', 'EDModel/')
Wrap = EQF(Stp, microIC,0.0,10.0)
output = Wrap.CPI()