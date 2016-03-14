import os 
import win32file
import datetime
import shutil
import subprocess
from time import clock, sleep

matlabcommand = "\"C:\\Program Files\\MATLAB\\R2011b\\bin\\matlab\" -nodesktop -nosplash -minimize "
currDir = os.getcwd()
# Overall output directory
baseDir = r"/tmp/eqfTestSuite/"
outputQueue = baseDir+"RunQueue/Runs-"+str(datetime.date.today())
outputDir = baseDir+"Results/Runs-"+str(datetime.date.today())

# Parameter List
NRunsList = [1000]
dtList = [2.0/12]
kList = [2]
MList = [0]
StepperStringList = ["euler"]
MacroStringList = ["covpar","dircomp","mean0"]
MacroStringDict = {"ssa":"MacroSSA","covpar":"CovariancePolynomial",
	"mean":"Mean","dircomp":"CovarianceRecompute","mean0":"CovarianceStabilized"}
RegimeList = ["endemic","vaccine"]
RegimeDict = {"endemic":"paramsEndemic", "vaccine":"paramsVaccinate"}

RunQueue = [] # List of folders to run
DirQueue = []
# Make Parameters file

# Copy contents of folder
def copyAllFiles(pathFrom,pathTo):
	filelist = os.listdir(pathFrom)
	
	for file in filelist:
		shutil.copy(pathFrom+file, pathTo+file)
	

def makeParametersFile(path, NRuns, dt, k, M, StepperString):
	def makeRunParametersString(NRuns, dt, k, M, StepperString):
		output = """
		cd('{5}');
NRuns = {0:d};
dt = {1:0.16f};
k = {2:d}; 
M = {3:0.16f}; 
StepperString = '{4}';""".format(NRuns, dt, k, M, StepperString,path)
		return output
		
	fid = open(path+"runParameters.m","w")
	fid.write(makeRunParametersString(NRuns, dt, k, M, StepperString))
	fid.close()
	
def makeCMS(path):
	copyAllFiles(baseDir+"CMSFiles/",path)
	copyAllFiles(baseDir+"EquationFreeFiles/",path)

def makeRegime(path, regime):
	copyAllFiles(baseDir+"ModelFiles/"+RegimeDict[regime]+"/",path)

def makeMacro(path, macro):
	copyAllFiles(baseDir+"MacroscaleFiles/"+MacroStringDict[macro]+"/",path);
	
def makeExecute(path):
	shutil.copy(baseDir+"ExecuteBatch/executeRun.m", path)
# Generate Queue Directory and Run Directory
try:
	os.mkdir(outputQueue)
	os.mkdir(outputDir)
except:
	pass

## --------------------------- Generate Simulation Directories --------------------------
print "-- Generating Simulation Directories"
start = clock()
for NRuns in NRunsList:
	for dt in dtList:
		for k in kList:
			for M in MList:
				for StepperString in StepperStringList:
					for Macrostring in MacroStringList:
						for Regime in RegimeList:
							try:
								path = outputQueue+"/"
								outPath = outputDir+"/"
								currDir = "run_{5}_{6}_{0:d}_{1:0.4f}_{2:d}_{3:0.4f}_{4}/".format(NRuns, dt, k, M,StepperString,Macrostring,Regime) 
								print "Making run with NRuns = {0:d}, dt = {1:0.4f}, k = {2:d}, M = {3:0.4f};".format( NRuns, dt, k, M, StepperString)
								os.mkdir(path+currDir)
								RunQueue.append((path+currDir).replace('/','\\'))
								DirQueue.append(currDir)
								# write parameters file
								makeParametersFile(path+currDir, NRuns, dt, k, M, StepperString)
								makeCMS(path+currDir)
								makeRegime(path+currDir,Regime)
								makeMacro(path+currDir, Macrostring)
								makeExecute(path+currDir)
							
							except:
								print "Error caught. Run skipped"

					

#os.system(matlabcommand+" -r C:{0}".format(RunQueue[0]+"executeRun"))
#print matlabcommand+" -r \"C:{0}\"".format(RunQueue[0]+"executeRun")
print "** Done in {0:f} seconds".format(clock()-start)
print "\n\n\n"
## --------------------------- Running Simulations ------------------------------------
for iter in range(0,len(RunQueue)):
	print "-- Starting run {0:d} of {1:d}".format(iter+1, len(RunQueue))
	start = clock()
	os.chdir("C:{0}".format(RunQueue[iter]))
	curr = subprocess.Popen(matlabcommand + " -r executeRun")
	while 1:
		try:
			with open("doneFile.dat") as f:
				f.close()
				break
		except:
			sleep(10)
	os.chdir(baseDir)
	done = False
	while not done:
		try:
			shutil.move(path+DirQueue[iter], outPath+DirQueue[iter])
			done = True
		except:
			pass
	print "** Finished run in {0:f} seconds".format(clock()-start) 
	