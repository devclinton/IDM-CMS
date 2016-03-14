function [ labels, data ] = RunModel(modelDescription, configFilename, solver, repeats, duration, samples)
%RUNMODEL Summary of this function goes here
%   Detailed explanation goes here

    try
        %tStart = tic;
        % TODO - better way to locate compartments.exe as this path can be
        % different for different modelers/developers
        disp('Assembly Time')
        tic
        NET.addAssembly('C:\SRC\TFSCMS\CMS\Main\framework\compartments\bin\x64\Release\compartments.exe');
        toc
        %tElapsed = toc(tStart);
        %fprintf('Assembly load time: %f seconds\n', tElapsed);
    catch exception
        if (strcmp(exception.identifier, 'MATLAB:NET:CLRException:AddAssembly'))
            throw(MException('CMS:RunModel', 'RunModel: Could not load compartments.exe'));
        else
            throw(exception);
        end
    end
    
    try
        %config = compartments.Configuration.ConfigurationFromString(configString);  
        config = compartments.Configuration(configFilename);
        disp('Load config time')
        tic
        NET.setStaticProperty('compartments.Configuration.CurrentConfiguration', config)
        toc
        
    catch exception
        if (strcmp(exception.identifier, 'MATLAB:NET:CLRException:MethodInvoke'))
            throw(MException('CMS:RunModel', 'RunModel: Error parsing configuration string.'));
        else
            throw(exception);
        end
    end

    try
    disp('Load model file time')
    tic
    model = compartments.emodl.EmodlLoader.LoadEMODLModel(modelDescription);
    toc
    %fprintf('Model parsing time: %f seconds\n', tElapsed);
    catch exception
        if (strcmp(exception.identifier, 'MATLAB:NET:CLRException:MethodInvoke'))
            throw(MException('CMS:RunModel', 'RunModel: Error loading model definition.'));
        else
            throw(exception);
        end
    end
    
    try
    %tStart = tic;
    disp('Execute Model Time')
    tic
    results = compartments.Program.ExecuteModel(model, solver, duration, repeats, samples);
    %compartments.Program.RunModel(model, solver, duration, repeats, samples);
    toc
    %tElapsed = toc(tStart);
    %fprintf('Simulation execution time: %f seconds\n', tElapsed);
    catch exception
        if (strcmp(exception.identifier, 'MATLAB:NET:CLRException:MethodInvoke'))
            throw(MException('CMS:RunModel', 'RunModel: Exception while executing simulation.'));
        else
            throw(exception);
        end
    end

    %tStart = tic;
    labels = results.Labels;
    trajectories = results.Data;
    nTrajectories = trajectories.Length;
    data = zeros(1,1);
    if (nTrajectories >= 1)
        nSamples = trajectories(1).Length;
        if (nSamples > 0)
            data = zeros(nTrajectories, nSamples);
            for t = 1:nTrajectories
                data(t,:) = trajectories(t);
            end
        end
    end
    %tElapsed = toc(tStart);
    %fprintf('Trajectory retrieval time: %f seconds\n', tElapsed);

end

