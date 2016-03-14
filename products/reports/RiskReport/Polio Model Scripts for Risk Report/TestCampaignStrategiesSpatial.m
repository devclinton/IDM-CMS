function TestCampaignStrategiesSpatial()
 
    addpath('utils')

    close all
    
    %%% Duration
    Years = 1;
    
    %%% Migration Matrix Needs to be square, diagonal terms not taken in
    %%% account, off diagonal terms are rates.
    
    pa.numNodes = 1;  
    
    rate = 0.01;
    pa.Mig = zeros(pa.numNodes);
    for kbar = 1:pa.numNodes
       if kbar == 1;
           pa.Mig(1,kbar+1) = rate ;
       elseif kbar == pa.numNodes;
           pa.Mig(pa.numNodes,kbar-1) = rate ;
       else 
           pa.Mig(kbar,kbar+1) = rate ;
           pa.Mig(kbar,kbar-1) = rate ;
       end
    end
           
    
    pa.MigStateFactor = [];            %Turn on, off, or modulate certain
                                        %compartments to migration
    
    %%% Campaign Parameters %%%
    
    pa.GlobalRoutineCoverage = .2;                      %Can be a single number (applied to all Nodes) or number for each Node
    pa.WhichAgeGroupsRecieveRoutine = [1];              %First entry is dose 0, subsequent entries denote vacc entry to age group
    pa.DropOffRateRoutinePerAgeGroup = [1];             %Factor for drop off rate for routine vaccs
    
    pa.VaccineEfficacy = 0.13;
    
    pa.VaccineEfficacyTOPVType1 = 0.13;
    pa.VaccineEfficacyTOPVType2 = 0.38;
    pa.VaccineEfficacyTOPVType3 = 0.20;
    
    pa.VaccineEfficacyBOPVType1 = 0.39;
    pa.VaccineEfficacyBOPVType3 = 0.48;
    pa.VaccineEfficacyMOPVType1 = 0.43;
    pa.VaccineEfficacyMOPVType3 = 0.53;
      
    % Type of Coverage. 
    
    pa.TypeOfCoverage = 'Standard';             %Standard Vs. Guillaume
    pa.TypeOfSIACampaign = 'Periodic';          %Periodic Vs. NonPeriodic
    
    pa.PeriodicCampaignSameForEachNode = 1;                %0 No, 1 Yes
    
    pa.PeriodicCampaignTiming.Node1 = [1/52 4/52 8/52 12/52];    %Timing of the when the periodic sias happen
    pa.GlobalSIACoverageVaccEff.Node1 = [1 2 3 4];     % 1= topv; 2 = bopv; 3 = mopv1 4 = mopv3

    pa.PeriodicCampaignTiming.Node2 = [1/52,3/52];    %Timing of the when the periodic sias happen
    pa.PeriodicCampaignTiming.Node3 = [1/52,7/52];    %Timing of the when the periodic sias happen
   
    pa.NonPeriodicCampaignSameForEachNode = 0;        %0 No, 1 Yes
    pa.NonPeriodicCampaignTiming.Node1 = [1/52,5/52];         % Put in the timing of the SIA Coverage
    pa.NonPeriodicCampaignTiming.Node2 = [2/52];
    pa.NonPeriodicCampaignTiming.Node3 = [6/52];

    pa.SIADuration = 0;
    pa.SIACoverageSameForAllNodes = 0;                %0 no, 1 yes
       
    pa.GlobalSIACoverage.Node1 = ones(1,length(pa.PeriodicCampaignTiming.Node1))*.3;

    pa.GlobalSIACoverage.Node2 = [0.8,0.8];                 % Coverage of each of the SIAs
    pa.GlobalSIACoverage.Node3 = [0.8,0.8];                 % Coverage of each of the SIAs
   
    % Accessibility Coverage
    
    pa.accessGroupPercent = [1];       %Amount of Population in each access group
    pa.accessGroupfactor =  [1];
    [pa.Cov] = ComputeCoverageEstimates(pa);
    
    param = [];
    
    %%% Stochastic Simulation Parameters %%%
    pa.WithinTrajSamples = 200;
    pa.NumTrajectories = 200;
    pa.Duration = Years;
    pa.Solver = 'TAU';
    
    pa.epsilon = 0.01;
               
    %%% Number of Accessibility Groups %%%
    pa.AccessibilityGroupNumbers = 1;           % Needs to match the access cov above
    pa.InfectedWildComp = 2;
    pa.HowManyWildTypes = 3;
    
    pa.InfectedVaccComp = 2;
    pa.numberOfAgeGroups = 1;
    pa.WhichCompAreInfectious = [0 1];
    
    pa.AgeMixing = 0;           % 0 no difference in mixing, 1 look for mixing matrix
    pa.AccessMixing = 0;        % 0 no difference in access mixing, 1 look for access matrix
    pa.AgingVector = [0];
    pa.DyingVector = [0.042];
    
    %Parameters
    param.betaWType1= 740;
    param.betaWType2= 740;
    param.betaWType3= 740;
    param.betaV= 40;
    param.alpha=0.1;
    param.N=10000;
 
    param.gamma=1/(28/365);
    param.nu=0.082;
    
    param.rec = 1/(2/52);
    
    param.DTMatrix = [-param.rec 0;              %% Disease compartment matrix rate W1 to W2 to... Wn
                      param.rec -param.rec;
                      0 param.rec];                

    % Create an IC or load your own.
    %IC = CreateIC(pa,param);

IC = [param.N;zeros(4*3*2,1);zeros(7,1);
    param.N;zeros(4*3*2,1);zeros(7,1)];
    
    close all

%%% Simulation time 
    pa.Duration = 10;
    
    [param,pa] = UpdateParameters(param,pa);

%%% Simulate using CMS
 figure
 
 CreateModelFile(round(IC),pa,param);
 CreateConfigFile(pa);
 
 [Labels,data] = StochasticSimulations(pa.Solver,[],pa,round(IC),param);
 
end


function [Labels,data] = StochasticSimulations(solver, ModelFileName,pa,IC,param)
    
    if isempty(ModelFileName)
        CreateModelFile(IC,pa,param);
        ModelFile = fileread('modelfile.emodl');
    else
        ModelFile = fileread(ModelFileName);
    end
    
    %configM = fileread('config.json');
    %TestConfig = parse_json(configM);
    
    [ Labels, data ] = RunModel(ModelFile, 'config.json', solver, pa.NumTrajectories, pa.Duration, pa.WithinTrajSamples);

    h = gca;
    plot(linspace(0,pa.Duration,pa.WithinTrajSamples),data','LineWidth',[1.5]);
    axis(h);
    set(gca,'FontSize',[22],'LineWidth',1.5)
    xlabel('time')
    ylabel('Population')
    title(['Stochastic Simulations'])

end

