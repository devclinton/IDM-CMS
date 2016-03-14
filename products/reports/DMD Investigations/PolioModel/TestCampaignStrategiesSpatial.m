function TestCampaignStrategiesSpatial()
 
addpath('utils')

close all
    %%% Simulation Time %%%
    Years = 20;
    
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
    
    pa.GlobalRoutineCoverage = .5;                                 %Can be a single number (applied to all Nodes) or number for each Node
    pa.WhichAgeGroupsRecieveRoutine = [1,1,1,1,0,0];            %First entry is dose 0, subsequent entries denote vacc entry to age group
    pa.DropOffRateRoutinePerAgeGroup = [0.6, 1, 0.6, 0.2,0,0];     %Factor for drop off rate for routine vaccs
    
    pa.VaccineEfficacy = 0.13;
      
    % Type of Coverage. 
    
    pa.TypeOfCoverage = 'Guillaume';             %Standard Vs. Guillaume
    pa.TypeOfSIACampaign = 'Periodic';          %Periodic Vs. NonPeriodic
    
    pa.PeriodicCampaignSameForEachNode = 1;                %0 No, 1 Yes
    
    
    % Two Versions of Campaign
    pa.PeriodicCampaignTiming.Node1 = [1/52,5/52,9/52,13/52,17/52,21/52,25/52,29/52,33/52,41/52,45/52];    %Timing of the when the periodic sias happen
    pa.GlobalSIACoverageVaccEff.Node1 = [0.39,0.39,0.39,0.39,0.39,0.13,0.13,0.13,0.13,0.13,0.13]; 
    
     pa.PeriodicCampaignTiming.Node1 = [1/52, 25/52];    %Timing of the when the periodic sias happen
     pa.GlobalSIACoverageVaccEff.Node1 = [0.99, 0.99];

    %pa.PeriodicCampaignTiming.Node1 = [1/52,7/52,13/52,19/52,25/52,31/52,37/52,48/52];    %Timing of the when the periodic sias happen
    pa.PeriodicCampaignTiming.Node2 = [1/52,3/52];    %Timing of the when the periodic sias happen
    pa.PeriodicCampaignTiming.Node3 = [1/52,7/52];    %Timing of the when the periodic sias happen
   
    pa.NonPeriodicCampaignSameForEachNode = 0;        %0 No, 1 Yes
    pa.NonPeriodicCampaignTiming.Node1 = [1/52,5/52];         % Put in the timing of the SIA Coverage
    pa.NonPeriodicCampaignTiming.Node2 = [2/52];
    pa.NonPeriodicCampaignTiming.Node3 = [6/52];

    pa.SIADuration = 4/365;
    pa.SIACoverageSameForAllNodes = 0;                %0 no, 1 yes
        
    pa.GlobalSIACoverage.Node1 = ones(1,length(pa.PeriodicCampaignTiming.Node1))*1/10;

    pa.GlobalSIACoverage.Node2 = [0.8,0.8];                 % Coverage of each of the SIAs
    pa.GlobalSIACoverage.Node3 = [0.8,0.8];                 % Coverage of each of the SIAs
   
    % Accessibility Coverage
    
    pa.accessGroupPercent = [.2 .4 .3 .1];       %Amount of Population in each access group
    pa.accessGroupfactor =  [1 .5 .2 .05];
    [pa.Cov] = ComputeCoverageEstimates(pa);
    
    param = [];
    
    %%% Stochastic Simulation Parameters %%%
    pa.WithinTrajSamples = 500;
    pa.NumTrajectories = 5;
    pa.Duration = Years;
    pa.Solver = 'TAU';
    
    pa.epsilon = 0.01;
               
    %%% Number of Accessibility Groups.
    pa.AccessibilityGroupNumbers = 4;           % Needs to match the access cov above
    pa.InfectedWildComp = 4;
    pa.InfectedVaccComp = 4;
    pa.numberOfAgeGroups = 5;
    pa.WhichCompAreInfectious = [0 0 1 1];
    
    pa.AgeMixing = 0;           % 0 no difference in mixing, 1 look for mixing matrix
    pa.AccessMixing = 0;        % 0 no difference in access mixing, 1 look for access matrix
    pa.AgingVector = [1/(6/52), 1/(4/52), 1/(4/52),1/(38/52),1/4];
    pa.DyingVector = [0.075; 0.075; 0.075; 0.075; 0.042];
    
    %Parameters
    param.betaW= 740;
    param.betaV= 40;
    param.alpha=0.1;
    param.N=400000;
    %param.mu=0.042;
    param.gamma=1/(28/365);
    param.nu=0.042;
    
    param.rec = 1/(1/52);
    
    param.DTMatrix = [-param.rec 0 0 0;              %% Disease compartment matrix rate W1 to W2 to... Wn
                      param.rec -param.rec 0 0;
                      0 param.rec -param.rec 0;
                      0 0 param.rec -param.rec;
                      0 0 0 param.rec];                
    

    % Create an IC or load your own.
    IC = CreateIC(pa,param);

    close all

    % Burn In
    pa.Duration = 10;
    
    [param,pa] = UpdateParameters(param,pa);
    
    CreateRHSfile(IC,pa);
    CreateConfigFile(pa);

    figure

disp('ODE simulation time')
tic
   [tsave,Xfsave] = HybridSimulations(pa,param,IC);
toc
FinalCondition = Xfsave(end,:);

%%%%
figure
pa.Duration = 4;
[param,pa] = UpdateParameters(param,pa);
FinalCondition(pa.AccessibilityGroupNumbers*pa.numberOfAgeGroups+2) = 5;

[tsave,Xfsave] = HybridSimulations(pa,param,FinalCondition);


 save FC FinalCondition
 
 CreateModelFile(round(FinalCondition),pa,param);
 CreateConfigFile(pa);
 
[Labels,data] = StochasticSimulations(pa.Solver,[],pa,round(FinalCondition),param);


    
end

function TimeToEradication(data,Labels,pa)

%%% Compute the time to Eradication for a data set

time = linspace(0,pa.Duration,pa.WithinTrajSamples);
histvec = zeros(pa.NumTrajectories,1);
vec = [];

for j = 1 : length(data(:,1))/5;
    
    vec = find(data(pa.NumTrajectories*2+j,1:end-1) == 0); 
    if isempty(vec)
        histvec(j) = 0;
    else
    histvec(j) = time(vec(1));
    end
    vec = [];
    
end

[N,X] = hist(histvec,100);

figure
bar(X,N/sum(N))
set(gca,'FontSize',[22],'LineWidth',[1.5])
xlabel('time')
ylabel('freq')

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

    subplot(1,2,1)
    h = gca;
    r = Labels.Length;
    for j = 1 : r
        
        tempString = char(Labels(j));
        
        if ~isempty(strfind(tempString,'InfectiousV'))
            figure(2)
        subplot(1, 2, 2); plot(linspace(0,pa.Duration,pa.WithinTrajSamples),data(j,:)','y','LineWidth',[1.5]), hold on;
        axis(h);
        set(gca,'FontSize',[22],'LineWidth',1.5)
        xlabel('time')
        ylabel('Population')
        title(['Stochastic Simulations'])
        elseif ~isempty(strfind(tempString,'InfectiousW'))
            figure(2)
        subplot(1, 2, 2); plot(linspace(0,pa.Duration,pa.WithinTrajSamples),data(j,:)','r','LineWidth',[1.5]), hold on;
        axis(h);
        set(gca,'FontSize',[22],'LineWidth',1.5)
        xlabel('time')
        ylabel('Population')
        title(['Stochastic Simulations'])
        elseif ~isempty(strfind(tempString,'su'))
            figure(2)
        subplot(1, 2, 2); plot(linspace(0,pa.Duration,pa.WithinTrajSamples),data(j,:)','b','LineWidth',[1.5]), hold on;
        axis(h);
        set(gca,'FontSize',[22],'LineWidth',1.5)
        xlabel('time')
        ylabel('Population')
        title(['Stochastic Simulations'])
        elseif ~isempty(strfind(tempString,'SI'))
        figure(3)
        plot(linspace(0,pa.Duration,pa.WithinTrajSamples),data(j,:)','g','LineWidth',[1.5]), hold on;
            
        else
        figure(2)
        subplot(1, 2, 2); plot(linspace(0,pa.Duration,pa.WithinTrajSamples),data(j,:)','g','LineWidth',[1.5]), hold on;
        axis(h);
        set(gca,'FontSize',[22],'LineWidth',1.5)
        xlabel('time')
        ylabel('Population')
        title(['Stochastic Simulations']) 
        end
    end
    
end

function [tsave,Xfsave] = HybridSimulations(pa,param,IC)
 
p=[];

Eventtimes = [];
StateMapping = [];
tsave = [];
Xfsave = [];

[tspanvec,SIANodeMatrix] = CreateHDStimebreaks(pa,param);

options = odeset('RelTol',1e-10,'AbsTol',1e-10);
Index = 1;

for j = 1:length(tspanvec(:,1))
    
    tspan = tspanvec(Index,:);
    
    ExIC = ExtractedSusc(IC,pa);
    
    g = ComputeRoutineCoverage(param,pa,IC);
    f = ComputeSIACoverage(param,pa,ExIC,Index,SIANodeMatrix); 
    
    disp(['Time is '])
    tspan(2)
    
    [tf,Xf]=ode15s(@RHSAccess,tspan,IC,options,param,pa,f,g,Eventtimes);
 
    Index = Index + 1;

    tsave = [tsave;tf];
    Xfsave = [Xfsave;Xf];
    IC=Xf(end,:)';   

end

n1 = pa.numberOfAgeGroups*pa.AccessibilityGroupNumbers;
n2 = pa.numberOfAgeGroups*pa.AccessibilityGroupNumbers+pa.numberOfAgeGroups*pa.InfectedWildComp;
n3 = pa.numberOfAgeGroups*pa.AccessibilityGroupNumbers+pa.numberOfAgeGroups*pa.InfectedWildComp + ...
    pa.numberOfAgeGroups*pa.InfectedVaccComp;

subplot(1,2,1), 
for j = 1 : length(Xfsave(1,:))
    
if j <= n1   
plot(tsave,Xfsave(:,j),'b'), hold on
elseif (j > n1 ) && (j <= n2)
    plot(tsave,Xfsave(:,j),'r'), hold on
elseif (j > n2 ) && (j <= n3)
    plot(tsave,Xfsave(:,j),'y'), hold on
else
    plot(tsave,Xfsave(:,j),'g'), hold on
end

end
set(gca,'FontSize',[22],'LineWidth',[1.5])
xlabel('time')
ylabel('Population')
axis([0 pa.Duration 0 max(max(Xfsave))])
title(['Hybrid ODE'])

FinalCondition = Xf(end,:);
sum(Xf(end,:))
sum(Xf(1,:))

end