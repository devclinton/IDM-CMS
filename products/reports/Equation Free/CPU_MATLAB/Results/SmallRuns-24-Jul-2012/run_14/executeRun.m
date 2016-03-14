clear all, close all, clc
runParameters; % additional script file with NRuns, dt, k, M

% Struct to pass to solver
solverData = struct('exec_path', ...
    'C:\src\CMS\compartments.exe',...
    'emodl_name', 'eqf_model.emodl', ...
    'solver', 'ssa', ...
    'config', 'config.json',...
    'reactions','eqfReactions.txt',...
    'parameters','eqfParams.txt',...
    'observables','eqfObserve.txt',...
    'nObservables',11, ...
    'SpeciesNames', {{'S','W11', 'W12', 'W21', 'W22', 'V11', 'V12', ...
    'V21', 'V22', 'N'}}, ...
    'ObservableNames', {{'t', 'S','W11', 'W12', 'W21', 'W22', 'V11', 'V12', ...
    'V21', 'V22', 'N'}},...
    'model_string', '');


%% 
Simulator = CMSWrapper(solverData);
Simulator.solverParams.duration = dt;
Funs = EQFScales();

load ICData.mat % Load initial conditions file, contains dataSet ICState with initial condition
dataIC = dataSet(dataIC);
ICState = Funs.MacroFromIC(dataIC);
state = ICState;

TimeStepper = Stepper(Simulator, Funs, k, M, NRuns); % Declare EQF wrapper
stateList = {};
stateList{1} = state;


% Main Computational Loop
for ii = 1:200
    state.t
    state = TimeStepper.Step(state);
    stateList{ii+1} = state;
    
    if state.t > 10
        break;
    end
end
ProcessResults;


save('OutputData.mat');

