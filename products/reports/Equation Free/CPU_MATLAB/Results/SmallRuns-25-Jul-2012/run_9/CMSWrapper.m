% Title  : cms_wrapper.m
% Author : Matt Williams (mawilliams@intven.com)
% Date   : 6/28/12
% Purpose: Updated version of the previous cms_wrapper.m. This is a MATLAB
%           class that serves as an interface between the .NET assembly
%           compartments.exe and MATLAB.  This code will load the assembly,
%           run single or multiple initial conditions, and interpret the
%           results.

classdef CMSWrapper < handle
    
    properties
        % solverData contains the information needed to generate a model and
        % interface with CMS.  These should not change even over multiple
        % runs
        solverData = struct('exec_path', ...
            'C:\src\CMS\framework\compartments\bin\x64\Release\compartments.exe',...
            'emodl_name', 'eqf_model.emodl', ...
            'solver', 'ssa', ...
            'config', 'config.json',...
            'reactions','test_vaccinate\eqf_reactions.txt',...
            'parameters','test_vaccinate\eqf_params.txt',...
            'observables','test_vaccinate\eqf_observe.txt',...
            'nObservables',11, ...
            'SpeciesNames', {{'S','W11', 'W12', 'W21', 'W22', 'V11', 'V12', ...
            'V21', 'V22', 'N'}}, ...
            'ObservableNames', {{'t', 'S','W11', 'W12', 'W21', 'W22', 'V11', 'V12', ...
            'V21', 'V22', 'N'}},...
            'model_string', '');
        
        % Solver parameters contains run specific information such as the
        % time-span of the simulation and the number of data points to save
        solverParams = struct('duration', 2/12,...
            'samples', 201);
        % Contains results from the CMS that need not be exposd to the user
        solverInterface = struct('config', [], ...
            'model', []);
        
    end
    
    methods
        % ---------------------------------------------------
        %                    Constructor
        % ---------------------------------------------------
        function obj = CMSWrapper(solverData, solverParams)
            % This code initializes the .NET assembly either from a given
            % list of solverData or from the default parameters
            
            if nargin > 0
                obj.solverData = solverData;
            end
            
            if nargin > 1
                obj.solverParams = solverParams;
            end
            
            %% Load .NET Assembly
            try
                NET.addAssembly(obj.solverData.exec_path);
                obj.solverInterface.config = compartments.Configuration(obj.solverData.config);
                NET.setStaticProperty('compartments.Configuration.CurrentConfiguration', ...
                    obj.solverInterface.config);
            catch exception
                if (strcmp(exception.identifier, 'MATLAB:NET:CLRException:MethodInvoke'))
                    throw(MException('CMS:RunModel', 'RunModel: Error parsing configuration string.'));
                else
                    throw(exception);
                end
            end
            
            % Generate Model Details
            ic_string = '';
            for ii = 1:length(obj.solverData.SpeciesNames)
                ic_string = [ic_string, sprintf('(species %s %d)\n',...
                    obj.solverData.SpeciesNames{ii}, 1)];
            end
            reactions_string = fileread(obj.solverData.reactions);
            observables_string = fileread(obj.solverData.observables);
            parameters_string = fileread(obj.solverData.parameters);
            
            obj.solverData.model_string = ...
                sprintf([...
                '(import (rnrs) (emodl cmslib))\n', ...
                '(start-model "seir.emodl")\n', ...
                ic_string, ...
                observables_string,...
                parameters_string,...
                reactions_string, ...
                sprintf('\n(end-model)\n')]);
            
            %% Load the model
            try
                obj.solverInterface.model = ...
                    compartments.emodl.EmodlLoader.LoadEMODLModel(obj.solverData.model_string);
            catch exception
                if (strcmp(exception.identifier, 'MATLAB:NET:CLRException:MethodInvoke'))
                    throw(MException('CMS:RunModel', 'RunModel: Exception while executing simulation.'));
                else
                    throw(exception);
                end
            end
        end
        
        % ---------------------------------------------------
        %                   Computing Functions
        % ---------------------------------------------------
        function dataout = execute_runs(obj, ICList)
            % Takes in a matrix of initial conditions with each new IC
            % in a column
            
            NRuns = size(ICList,2); % Compute number of runs from IC List
            
            try
                % Ignore tack on an extra data point so the 0 bug won't
                % impact the results I need
                dt = obj.solverParams.duration/(obj.solverParams.samples-1);
                newDuration = obj.solverParams.duration+dt;
                newSamples = obj.solverParams.samples+1;
                % Call .NET Assembly
                obj.solverInterface.model = ...
                    compartments.emodl.EmodlLoader.LoadEMODLModel(obj.solverData.model_string);
                results = compartments.Program.ExecuteModelWithIC(...
                    obj.solverInterface.model,...
                    obj.solverData.solver,...
                    newDuration,...
                    NRuns, newSamples,...
                    ICList);
                
            catch exception
                if (strcmp(exception.identifier, 'MATLAB:NET:CLRException:MethodInvoke'))
                    throw(MException('CMS:RunModel', 'RunModel: Exception while executing simulation.'));
                else
                    throw(exception);
                end
            end
            
            trajectories = results.Data;
            nTrajectories = trajectories.Length;
            dataout = {};
            
            if (nTrajectories>0)
                for ii = 1:nTrajectories/NRuns
                    nSamples = trajectories(1).Length;
                    if (nSamples > 0)
                        dataout{ii} = zeros(NRuns,nSamples);
                        
                        for jj = 1:NRuns
                            dataout{ii}(jj,:) = trajectories(jj + NRuns*(ii-1));
                        end
                    end
                end
            end
            
            for ii = 1:length(dataout)
                dataout{ii} = dataout{ii}(:,1:end-1); % Remove last point
            end
            
        end
        
        function output = step(obj, ICSet)
               ICList = ICSet.data;
               NRuns = size(ICList,2); % Compute number of runs from IC List
            try
                % Ignore tack on an extra data point so the 0 bug won't
                % impact the results I need

                % Call .NET Assembly
                obj.solverInterface.model = ...
                    compartments.emodl.EmodlLoader.LoadEMODLModel(obj.solverData.model_string);
                results = compartments.Program.StepModelWithIC(...
                    obj.solverInterface.model,...
                    obj.solverData.solver,...
                    obj.solverParams.duration,...
                    NRuns, obj.solverParams.samples,...
                    ICList);
                
            catch exception
                if (strcmp(exception.identifier, 'MATLAB:NET:CLRException:MethodInvoke'))
                    throw(MException('CMS:RunModel', 'RunModel: Exception while executing simulation.'));
                else
                    throw(exception);
                end
            end
           
            output = dataSet(zeros(size(ICList)));
            output.time = obj.solverParams.duration;
            for ii = 1:size(ICList,1)
                for jj = 1:size(ICList,2)
                output.data(ii,jj) = (results(ii,jj));
                end
            end
            
        end
        
    end
end

