# command line arguments:
#     executable path and name (e.g. c:\src\cms\framework\compartments.exe)
#     results destination directory

import sys
import json
import tempfile
import os
import subprocess

class TestConfiguration:
    
    def __init__(self, executablePath, outputPath, solverSet):
        self.executablePath = executablePath
        self.outputPath     = outputPath
        self.solverSet      = solverSet
        return

class TestInstance:
    
    def __init__(self, model, templateText):
        self.modelFilename  = model['model']
        self.configFilename = model['config']
        self.solvers        = model['solvers']
        self.templateText   = templateText
        return

    @property
    def modelName(self):
        (path, name) = os.path.split(self.modelFilename)
        return name
    
    @property
    def configName(self):
        (path, name) = os.path.split(self.configFilename)
        return name

def UsingSolver(solver, solverSet):
    using = True
    if (solverSet != None):
        using = solver in solverSet
        
    return using

def PrintSolverSet(solverSet):
    if (solverSet == None):
        print "Using all specified solvers."
    else:
        print "Using solvers ",
        for s in solverSet:
            print s + " ",
        print
        
def LoadTestSuite(testSuite):
    # open testSuite
    with open(testSuite) as suiteFile:
        suiteData = json.load(suiteFile)
    return suiteData

def ValidateTestSuite(suiteData):
    # verify presence of each model file and config template
    isValid = True
    for model in suiteData['models']:
    
        # verify model file
        modelFileName = model['model']
        if not os.path.exists(modelFileName):
            isValid = False
            print "Couldn't find model file '" + modelFileName + "'."

        # verify config template file            
        configFileName = model['config']
        if not os.path.exists(configFileName):
            isValid = False
            print "Couldn't find config file '" + configFileName + "'."
            
    return isValid

def GetModelName(model):
    # identify model file
    modelFileName = model['model']
    print 'Model name ' + modelFileName
    (path, name) = os.path.split(modelFileName)
    (modelName, extension) = os.path.splitext(name)
    return (modelFileName, modelName)

def GetConfigTemplateName(model):
    # identify config template file
    configFileName = model['config']
    print 'Config template ' + configFileName
    (path, name) = os.path.split(configFileName)
    (configName, extension) = os.path.splitext(name)
    return (configFileName, configName)

def ReadConfigTemplateText(configFileName):
    # load config template
    with open(configFileName) as configFile:
        templateText = configFile.read()
    return templateText

def BuildResultsFilePrefix(testConfiguration, testInstance, solver):
    # construct test output root filename
    resultsFilePrefix = testConfiguration.outputPath + '\\' + testInstance.modelName + '-' + testInstance.configName + '-' + solver
    return resultsFilePrefix

def WriteConfigToFile(configText):
    # write working config to temp file
    (fileDescriptor, tempFileName) = tempfile.mkstemp(suffix='.cfg')
    with os.fdopen(fileDescriptor, 'w') as configFile:
        configFile.write(configText)
        
    return tempFileName

def ExecuteSolverOnModel(testConfiguration, testInstance, resultsFileRoot, configFilename):
    # execute compartments.exe
    # capture output in log file
    with open(resultsFileRoot + '.log.txt', 'w') as outputFile:
        #outputFile = open(resultsFileRoot + '.log.txt', 'w')
        with open(resultsFileRoot + '.err.txt', 'w') as errorFile:
            #errorFile = open(resultsFileRoot + '.err.txt', 'w')
            subprocess.call([testConfiguration.executablePath, "-model", testInstance.modelFilename, "-config", configFilename], stdout=outputFile, stderr=errorFile)
            #outputFile.close()
            #errorFile.close()

def CleanUpConfigFile(configFilename):
    # clean up
    os.remove(configFilename)

def RenameResults(resultsFilePrefix):
    # TODO handle overwriting of results
    # TODO handle comparison of results with existing results
    
    # rename results (trajectories.csv) to model-config-solver.csv and move to destination directory
    if os.path.exists('trajectories.csv'):
        destination = resultsFilePrefix + '.csv'
        if os.path.exists(destination):
            os.remove(destination)
        os.rename('trajectories.csv', destination)
    elif os.path.exists('trajectories.txt'):
        destination = resultsFilePrefix + '.txt'
        if os.path.exists(destination):
            os.remove(destination)
        os.rename('trajectories.txt', destination)
    else:
        print '!!! No results file found !!!'

def RunSolvers(testConfiguration, testInstance):
    # for each item in "solvers" array
    for solver in testInstance.solvers:
    
        if UsingSolver(solver, testConfiguration.solverSet):
            print 'Using solver ' + solver + '...',
        
            # construct test output root filename
            resultsFileRoot = BuildResultsFilePrefix(testConfiguration, testInstance, solver)
        
            # replace $SOLVER$ in config template with solver
            configText = testInstance.templateText.replace('$SOLVER$', solver)
            
            tempFilename = WriteConfigToFile(configText)
            ExecuteSolverOnModel(testConfiguration, testInstance, resultsFileRoot, tempFilename)
            CleanUpConfigFile(tempFilename)            
            print 'finished. Renaming results to ' + testConfiguration.outputPath
            RenameResults(resultsFileRoot)

def RunModels(testConfiguration, suiteData):
    # for each item in "models" array
    for model in suiteData['models']:

        (modelFileName,  modelName)  = GetModelName(model)
        (configFileName, configName) = GetConfigTemplateName(model)
        templateText = ReadConfigTemplateText(configFileName)
        
        testInstance = TestInstance(model, templateText)
        
        RunSolvers(testConfiguration, testInstance)

def CleanUp():
    # clean up
    if os.path.exists('trajectories.json'):
        os.remove('trajectories.json')

def RunTests(testConfiguration, testSuite):
    "RunTests documentation..."
    
    print "Running '" + testConfiguration.executablePath + "' on tests in '" + testSuite + "'."
    PrintSolverSet(testConfiguration.solverSet)

    suiteData = LoadTestSuite(testSuite)

    if not ValidateTestSuite(suiteData):
        return
    
    RunModels(testConfiguration, suiteData)

    CleanUp()
    
    print 'Finished running tests.'
    return

def ProcessCommandlineArguments(argv):
    
    executablePath = argv[1]
    testMatrixFile = argv[2]
    outputPath     = argv[3]
    argc = len(argv)
    if (argc > 4):
        solverSet = set()
        for i in range(4, argc):
            print "Adding " + argv[i] + " to solver set."
            solverSet.add(argv[i])
    else:
        solverSet = None
    
    return (executablePath, testMatrixFile, outputPath, solverSet)

if __name__ == '__main__':

    argc = len(sys.argv)

    # TODO optionally process "overwrite" flag to allow overwriting existing results
    # TODO optionally process "compare" option with directory specification

    if argc >= 4:
        (executablePath, testMatrix, outputPath, solverSet) = ProcessCommandlineArguments(sys.argv)
        testConfiguration = TestConfiguration(executablePath, outputPath, solverSet)
        RunTests(testConfiguration, testMatrix)
    else:
        # extract script name from first command line argument
        (path, script) = os.path.split(sys.argv[0])
        print 'Usage: ' + script + ' executable testsuite results-directory [solver [solver [...]]]'
