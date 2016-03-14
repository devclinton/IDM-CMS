function [newparam,pa] = UpdateParameters(param,pa)

newparam = param;

%% Change the transmission parameter beta depending on the input by user

if size(param.betaWType1) == 1    
    %NewBetaW = param.betaW * ones(pa.numberOfAgeGroups,pa.numberOfAgeGroups,pa.AccessibilityGroupNumbers,pa.AccessibilityGroupNumbers);   
    %newparam.betaW = NewBetaW;
        NewBetaWType1 = param.betaWType1 * ones(pa.numberOfAgeGroups,pa.numberOfAgeGroups);   
    newparam.betaWType1 = NewBetaWType1;
end

if size(param.betaWType2) == 1    
    %NewBetaW = param.betaW * ones(pa.numberOfAgeGroups,pa.numberOfAgeGroups,pa.AccessibilityGroupNumbers,pa.AccessibilityGroupNumbers);   
    %newparam.betaW = NewBetaW;
        NewBetaWType2 = param.betaWType2 * ones(pa.numberOfAgeGroups,pa.numberOfAgeGroups);   
    newparam.betaWType2 = NewBetaWType2;
end

if size(param.betaWType3) == 1    
    %NewBetaW = param.betaW * ones(pa.numberOfAgeGroups,pa.numberOfAgeGroups,pa.AccessibilityGroupNumbers,pa.AccessibilityGroupNumbers);   
    %newparam.betaW = NewBetaW;
        NewBetaWType3 = param.betaWType3 * ones(pa.numberOfAgeGroups,pa.numberOfAgeGroups);   
    newparam.betaWType3 = NewBetaWType3;
end

%%% Ignore for now
% if size(param.betaV) == 1    
%     %NewBetaV = param.betaV * ones(pa.numberOfAgeGroups,pa.numberOfAgeGroups,pa.AccessibilityGroupNumbers,pa.AccessibilityGroupNumbers);   
%     %newparam.betaV = NewBetaV;
%         NewBetaV = param.betaV * ones(pa.numberOfAgeGroups,pa.numberOfAgeGroups);   
%     newparam.betaV = NewBetaV;
% end
% 
% if (length(param.betaW(:,1)) == pa.numberOfAgeGroups) && (pa.AgeMixing == 1)    
%     NewBetaW = zeros(pa.NumberOfAgeGroups,pa.NumberOfAgeGroups,pa.AccessibilityGroupNumbers,pa.AccessibilityGroupNumbers); 
%     
%     for j = 1 : numberOfAgeGroups
%        for jj = 1:numberOfAgeGroups
%            for jjj = 1:AccessibilityGroupNumbers
%                for jjjj = 1:AccessibilityGroupNumbers
%                 NewBetaW(j,jj,jjj,jjjj) = param.betaW(j,jj);
%                end
%            end
%        end
%     end
%           
%     newparam.betaW = NewBetaW;
% elseif (length(param.betaW(:,1)) ~= pa.numberOfAgeGroups) && (pa.AgeMixing == 1)  
%     disp('Error between size of beta matrix and number of AgeGroups')
%     return
% end
% 
% if (length(param.betaV(:,1)) == pa.numberOfAgeGroups) && (pa.AgeMixing == 1)
% NewBetaV = zeros(pa.NumberOfAgeGroups,pa.NumberOfAgeGroups,pa.AccessibilityGroupNumbers,pa.AccessibilityGroupNumbers); 
%     
%     for j = 1 : numberOfAgeGroups
%        for jj = 1:numberOfAgeGroups
%            for jjj = 1:AccessibilityGroupNumbers
%                for jjjj = 1:AccessibilityGroupNumbers
%                 NewBetaV(j,jj,jjj,jjjj) = param.betaV(j,jj);
%                end
%            end
%        end
%     end
%           
%     newparam.betaV = NewBetaV;
%     
% elseif (length(param.betaV(:,1)) ~= pa.numberOfAgeGroups) && (pa.AgeMixing == 1) 
%     disp('Error between size of beta matrix and number of AgeGroups')
%     return
% end
% 
% if (length(param.betaW(:,1)) == pa.AccessibilityGroupNumbers) && (pa.AccessMixing == 1)    
%     NewBetaW = zeros(pa.NumberOfAgeGroups,pa.NumberOfAgeGroups,pa.AccessibilityGroupNumbers,pa.AccessibilityGroupNumbers); 
%     
%     for j = 1 : numberOfAgeGroups
%        for jj = 1:numberOfAgeGroups
%            for jjj = 1:AccessibilityGroupNumbers
%                for jjjj = 1:AccessibilityGroupNumbers
%                 NewBetaW(j,jj,jjj,jjjj) = param.betaW(jjj,jjjj);
%                end
%            end
%        end
%     end
%           
%     newparam.betaW = NewBetaW;
% elseif (length(param.betaW(:,1)) ~= pa.AccessibilityGroupNumbers) && (pa.AccessMixing == 1) 
%     disp('Error between size of beta matrix and number of AccessMixing')
%     return
% end
% 
% if (length(param.betaV(:,1)) == pa.AccessibilityGroupNumbers) && (pa.AccessMixing == 1) 
% NewBetaV = zeros(pa.NumberOfAgeGroups,pa.NumberOfAgeGroups,pa.AccessibilityGroupNumbers,pa.AccessibilityGroupNumbers); 
%     
%     for j = 1 : pa.numberOfAgeGroups
%        for jj = 1:pa.numberOfAgeGroups
%            for jjj = 1:AccessibilityGroupNumbers
%                for jjjj = 1:AccessibilityGroupNumbers
%                 NewBetaV(j,jj,jjj,jjjj) = param.betaV(jjj,jjjj);
%                end
%            end
%        end
%     end
%           
%     newparam.betaV = NewBetaV;
%     
% elseif (length(param.betaV(:,1)) ~= pa.AccessibilityGroupNumbers) && (pa.AccessMixing == 1) 
%     disp('Error between size of beta matrix and number of AccessMixing')
%     return
% end

%% Routine Coverage

[u,v] = size(pa.GlobalRoutineCoverage);

if (u == 1) && (v == 1)  
    pa.GlobalRoutineCoverageVec = pa.GlobalRoutineCoverage * ones(1,pa.numNodes);       
end

%%  Modulate the Aging Matrix 

IC = [];
g = ComputeRoutineCoverage(param,pa,IC);

for jj = 1 : pa.numNodes
    
    eval(['gtemp = g.Node',num2str(jj),'(:);']);
    AgingMatrix = zeros(pa.numberOfAgeGroups,pa.numberOfAgeGroups);
    RoutineAgingMatrix = zeros(pa.numberOfAgeGroups,pa.numberOfAgeGroups);
    NormalAgingMatrix = zeros(pa.numberOfAgeGroups,pa.numberOfAgeGroups);
    
    for jjj = 1:pa.AccessibilityGroupNumbers
        if pa.numberOfAgeGroups ~= 1
            for j = 1 : pa.numberOfAgeGroups
               
                if j == pa.numberOfAgeGroups
                    AgingMatrix(j,j-1) = (1-pa.VaccineEfficacy*gtemp(jjj)*pa.DropOffRateRoutinePerAgeGroup(j+1))*pa.AgingVector(j-1);
                    RoutineAgingMatrix(j,j-1) = (pa.VaccineEfficacy*gtemp(jjj)*pa.DropOffRateRoutinePerAgeGroup(j+1))*pa.AgingVector(j-1);
                    NormalAgingMatrix(j,j-1) = pa.AgingVector(j-1);
                    
                    AgingMatrix(j,j) = -pa.AgingVector(j); 
                    NormalAgingMatrix(j,j) = -pa.AgingVector(j);
                elseif j == 1 
                    AgingMatrix(j,j) = - pa.AgingVector(j);
                    NormalAgingMatrix(j,j) = - pa.AgingVector(j);
                else
                    AgingMatrix(j,j-1) = (1-pa.VaccineEfficacy*gtemp(jjj)*pa.DropOffRateRoutinePerAgeGroup(j+1))*pa.AgingVector(j-1);
                    AgingMatrix(j,j) = - pa.AgingVector(j);
                    RoutineAgingMatrix(j,j-1) = (pa.VaccineEfficacy*gtemp(jjj)*pa.DropOffRateRoutinePerAgeGroup(j+1))*pa.AgingVector(j-1);
                    NormalAgingMatrix(j,j-1) = pa.AgingVector(j-1);
                    NormalAgingMatrix(j,j) = - pa.AgingVector(j);
                
                end      
            end
        end
    end 
    
    eval(['newparam.AgingMatrixNode',num2str(jj),' = AgingMatrix;'])
    eval(['newparam.RoutineAgingMatrixNode',num2str(jj),' = RoutineAgingMatrix;'])
    clear AgingMatrix RoutineAgingMatrix
    
end
    eval(['newparam.AgingMatrix = NormalAgingMatrix;'])

%% Birth Vec


BirthAgeVec = zeros(pa.numberOfAgeGroups,1);
BirthAgeVec(1) = 1;

newparam.BirthAgeVec = BirthAgeVec;

%% State Migration Rate.

if ~isfield(param,'StateMigRate')
   nn = length(pa.Mig(:,1)); 
   newparam.StateMigRate = ones(nn*pa.AccessibilityGroupNumbers*pa.numberOfAgeGroups+nn*pa.InfectedWildComp*pa.numberOfAgeGroups*3*5 ...
       +nn*7,1); 
    
end

%% Birth

if length(param.nu) == 1
   
    newparam.nu = param.nu * ones(length(pa.Mig(:,1)),1);
    
end

%% Population for each Node
if length(param.N) == 1   
    newparam.N = param.N * ones(length(pa.Mig(:,1)),1);
end



%% Accessibility
[u,v] = size(pa.accessGroupPercent);

if (u == 1) && (v == 1)
    newparam.accessGroupPercent = pa.accessGroupPercent * ones(length(pa.Mig(:,1)),1);
elseif (u == 1) && (v ~= 1)
    for j = 1:length(pa.Mig(:,1))
        newparam.accessGroupPercent(j,:) = pa.accessGroupPercent;
    end
end

%% Uniformly address the SIA input parameters



if strcmp(pa.TypeOfSIACampaign,'Periodic')
    
    for j = 1 : pa.numNodes
    eval(['pa.GlobalSIACoverageVec.Node',num2str(j),' = [];']);
    eval(['pa.PeriodicCampaignTimingVec.Node',num2str(j),' = [];']);
    eval(['pa.GlobalSIACoverageVaccEffVec.Node',num2str(j),' = [];']);
    end

    if pa.PeriodicCampaignSameForEachNode == 1
        temp = pa.GlobalSIACoverage.Node1;
        temp2 = pa.PeriodicCampaignTiming.Node1;
        temp3 = pa.GlobalSIACoverageVaccEff.Node1;
        
        for j = 1 : ceil(pa.Duration)
            pa.GlobalSIACoverageVec.Node1 = [pa.GlobalSIACoverageVec.Node1 temp];
            pa.PeriodicCampaignTimingVec.Node1 = [pa.PeriodicCampaignTimingVec.Node1 temp2+(j-1)];
            pa.GlobalSIACoverageVaccEffVec.Node1 = [pa.GlobalSIACoverageVaccEffVec.Node1 temp3];
        end
        for j = 2 : pa.numNodes
           eval(['pa.GlobalSIACoverageVec.Node',num2str(j),' = pa.GlobalSIACoverageVec.Node1;']);
           eval(['pa.PeriodicCampaignTimingVec.Node',num2str(j),' = pa.PeriodicCampaignTimingVec.Node1;']);
           eval(['pa.GlobalSIACoverageVaccEffVec.Node',num2str(j),' = pa.GlobalSIACoverageVaccEffVec.Node1;']);
        end
          
    else
        
        for j = 1 : ceil(pa.Duration)
            for jj = 1 : pa.numNodes
                
                eval(['temp = pa.GlobalSIACoverage.Node',num2str(jj),';']);
                eval(['temp2 = pa.PeriodicCampaignTiming.Node',num2str(jj),';']);
                eval(['temp3 = pa.GlobalSIACoverageVaccEff.Node',num2str(jj),';']);
                
                eval(['pa.GlobalSIACoverageVec.Node',num2str(jj),' = [pa.GlobalSIACoverageVec.Node',num2str(jj),' temp];']);
                eval(['pa.PeriodicCampaignTimingVec.Node',num2str(jj),' = [pa.PeriodicCampaignTimingVec.Node',num2str(jj),' temp2+(j-1)];']);
                eval(['pa.GlobalSIACoverageVaccEffVec.Node',num2str(jj),' = [pa.GlobalSIACoverageVaccEffVec.Node',num2str(jj),' temp3];']);
            
            end
        end
        
        
    end
end

if strcmp(pa.TypeOfSIACampaign,'NonPeriodic')
    
    for j = 1 : pa.numNodes
    eval(['pa.GlobalSIACoverageVec.Node',num2str(j),' = [];']);
    eval(['pa.NonPeriodicCampaignTimingVec.Node',num2str(j),' = [];']);
    eval(['pa.GlobalSIACoverageVaccEffVec.Node',num2str(j),' = [];']);
    end
    
    if pa.NonPeriodicCampaignSameForEachNode == 1
        
        pa.GlobalSIACoverageVec.Node1 = pa.GlobalSIACoverage.Node1;
        pa.NonPeriodicCampaignTimingVec.Node1 = pa.NonPeriodicCampaignTiming.Node1;
        pa.GlobalSIACoverageVaccEffVec.Node1 = pa.GlobalSIACoverageVaccEff.Node1;
        
        for j = 2 : pa.numNodes
           eval(['pa.GlobalSIACoverageVec.Node',num2str(j),' = pa.GlobalSIACoverage.Node1;']);
           eval(['pa.NonPeriodicCampaignTimingVec.Node',num2str(j),' = pa.NonPeriodicCampaignTiming.Node1;']);
           eval(['pa.GlobalSIACoverageVaccEffVec.Node',num2str(j),' = pa.GlobalSIACoverageVaccEff.Node1;']);
        end
    else
       for j = 1 : pa.numNodes
           eval(['pa.GlobalSIACoverageVec.Node',num2str(j),' = pa.GlobalSIACoverage.Node',num2str(j),';']);
           eval(['pa.NonPeriodicCampaignTimingVec.Node',num2str(j),' = pa.NonPeriodicCampaignTiming.Node',num2str(j),';']);
           eval(['pa.GlobalSIACoverageVaccEffVec.Node',num2str(j),' = pa.GlobalSIACoverageVaccEff.Node',num2str(j),';']);
        end
        
        
        
    end
    
    
    
end

%Count the number of SIAs
Count = 0;
pa.numOfSIAsPerNode = zeros(1,pa.numNodes);
for j = 1 : pa.numNodes
   eval(['Count = Count + length(pa.GlobalSIACoverageVec.Node',num2str(j),'(:));']); 
   eval(['pa.numOfSIAsPerNode(j) = length(pa.GlobalSIACoverageVec.Node',num2str(j),'(:));']);
end
pa.numOfSIAs = Count;


%% Death

if length(pa.DyingVector) == 1
   
    pa.DyingVector = ones(length(pa.AgingVector),1) * pa.DyingVector(1);
    
end


end