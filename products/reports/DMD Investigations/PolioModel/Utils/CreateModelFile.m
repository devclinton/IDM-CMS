function output = CreateModelFile(IC,pa,param)

%%%%  This function creates an emodl file for the framework

%% Initialization of the emodl file

Equations(1,:)={['(import (rnrs) (emodl cmslib))']};
Equations(end+1,:) = {['(start-model "Campaign")']};

numberOfNodes = length(pa.Mig(:,1));
m = 1;

%% Initialize all of the Species m keeps track of how many species in order to pull from the Initical Condition IC vector

for jj = 1:numberOfNodes

    Equations(end+1,:) = {['']};
    Equations(end+1,:) = {[';(locale site-Node',num2str(jj),')']};
    Equations(end+1,:) = {[';(set-locale site-',num2str(jj),')']};
    Equations(end+1,:) = {['']};
    
    for j = 1:pa.AccessibilityGroupNumbers
        for k = 1:pa.numberOfAgeGroups
            Equations(end+1,:) = {['(species Node',num2str(jj),'::S',num2str(j),'Age',num2str(k),' ',num2str(IC(m)),')']};
            m = m+1;
        end
    end

    %for jjj = 1:pa.AccessibilityGroupNumbers
        for j = 1:pa.InfectedWildComp
            for k = 1:pa.numberOfAgeGroups
                Equations(end+1,:) = {['(species Node',num2str(jj),'::W',num2str(j),'Age',num2str(k),' ',num2str(IC(m)),')']};
                m = m+1;
            end
        end
    %end

    %for jjj = 1:pa.AccessibilityGroupNumbers
        for j = 1:pa.InfectedVaccComp
            for k = 1:pa.numberOfAgeGroups
                Equations(end+1,:) = {['(species Node',num2str(jj),'::V',num2str(j),'Age',num2str(k),' ',num2str(IC(m)),')']};
                m = m+1;
            end
        end
    %end

    for k = 1:pa.numberOfAgeGroups
        Equations(end+1,:) = {['(species Node',num2str(jj),'::RAge',num2str(k),' ',num2str(IC(m)),')']};
        m = m+1;
    end

    Equations(end+1,:) = {['']};

end

%% Initialize all of the Observables

for jj = 1:numberOfNodes
        
    for j = 1:pa.AccessibilityGroupNumbers
        for k = 1:pa.numberOfAgeGroups
            Equations(end+1,:) = {['(observe susceptibleNode',num2str(jj),'S',num2str(j),'Age',num2str(k), ...
            ' Node',num2str(jj),'::S',num2str(j),'Age',num2str(k),')']};
        end
    end

    %for jjj = 1:pa.AccessibilityGroupNumbers
        for j = 1:pa.InfectedWildComp
            for k = 1:pa.numberOfAgeGroups
                Equations(end+1,:) = {['(observe InfectiousWildNode',num2str(jj),'W',num2str(j),'Age',num2str(k), ...
                ' Node',num2str(jj),'::W',num2str(j),'Age',num2str(k),')']};
            end
        end
    %end

    %for jjj = 1:pa.AccessibilityGroupNumbers
        for j = 1:pa.InfectedVaccComp
            for k = 1:pa.numberOfAgeGroups
                Equations(end+1,:) = {['(observe InfectiousVaccNode',num2str(jj),'V',num2str(j),'Age',num2str(k), ...
                ' Node',num2str(jj),'::V',num2str(j),'Age',num2str(k),')']};
            end
        end
    %end


    for k = 1:pa.numberOfAgeGroups
        Equations(end+1,:) = {['(observe RNode',num2str(jj),'RAge',num2str(k), ...
        ' Node',num2str(jj),'::RAge',num2str(k),')']};
    end

    Equations(end+1,:) = {['']};

end



%% This section enumerates different parameters by accessing specific parameters

%Aging
for j = 1:pa.numberOfAgeGroups
    if length(pa.AgingVector)==1
        Equations(end+1,:) = {['(param alphaAge',num2str(j),' ',num2str(pa.AgingVector),')']};
    else
        Equations(end+1,:) = {['(param alphaAge',num2str(j),' ',num2str(pa.AgingVector(j)),')']};
    end
end
Equations(end+1,:) = {['']};

%Death
for j = 1 : pa.numberOfAgeGroups
    
    Equations(end+1,:) = {['(param muAge',num2str(j),' ',num2str(pa.DyingVector(j)),')']};
    
end
Equations(end+1,:) = {['']};

%Recovery
Equations(end+1,:) = {['(param gamma ',num2str(param.gamma),')']};
Equations(end+1,:) = {['']};

%Birth
for j = 1 : numberOfNodes
    if length(param.nu) == 1
       Equations(end+1,:) = {['(param Node',num2str(j),'nu ',num2str(param.nu),')']};
    else
       Equations(end+1,:) = {['(param Node',num2str(j),'nu ',num2str(param.nu(j)),')']};
    end    
end
Equations(end+1,:) = {['']};

%Population of each of the Nodes
for j = 1 : numberOfNodes
    if length(param.N) == 1
       Equations(end+1,:) = {['(param Node',num2str(j),'N ',num2str(param.N),')']};
    else
       Equations(end+1,:) = {['(param Node',num2str(j),'N ',num2str(param.N(j)),')']};
    end   
end
Equations(end+1,:) = {['']};
Equations(end+1,:) = {['(param VaccineEfficacy ',num2str(pa.VaccineEfficacy),')']};
Equations(end+1,:) = {['']};

%Routine Vaccination for each Node for each accessibility group
%This parameter initialization needs more effort.  Need to take global
%coverage and convert it to a routine number for each access,node

g = ComputeRoutineCoverage(param,pa,IC);

for j = 1:numberOfNodes    
    for jj = 1:pa.AccessibilityGroupNumbers       
        eval(['F = g.Node',num2str(j),'(',num2str(jj),');']);        
        Equations(end+1,:) = {['(param Node',num2str(j),'gRoutAccess',num2str(jj),' ',num2str(F),')']};
    end
end
Equations(end+1,:) = {['']};

% for j = 1:numberOfNodes
%     for jj = 1:pa.AccessibilityGroupNumbers
%         if length(pa.RoutineCoverage)==1
%             Equations(end+1,:) = {['(param Node',num2str(j),'SIAAccess',num2str(jj),' ',num2str(pa.accessToSIAs(jj)),')']};
%         elseif length(pa.RoutineCoverage) == pa.AccessibilityGroupNumbers
%             Equations(end+1,:) = {['(param Node',num2str(j),'SIAAccess',num2str(jj),' ',num2str(pa.accessToSIAs(jj)),')']};
%         else
%             Equations(end+1,:) = {['(param Node',num2str(j),'SIAAccess',num2str(jj),' ',num2str(pa.accessToSIAs(j,jj)),')']};   
%         end
%     end
% end
% Equations(end+1,:) = {['']};

%for jjj = 1:pa.AccessibilityGroupNumbers
    for kkk = 1:pa.numberOfAgeGroups    
        for k = 1:pa.numberOfAgeGroups      
            %for jjjj = 1:pa.AccessibilityGroupNumbers    
        
                Equations(end+1,:) = {['(param BetaWAge',num2str(kkk),num2str(k),' ',num2str(param.betaW(kkk,k)),' ) ']};
            %end        
        end      
    end   
%end
Equations(end+1,:) = {['']};
    
%for jjj = 1:pa.AccessibilityGroupNumbers
    for kkk = 1:pa.numberOfAgeGroups    
        for k = 1:pa.numberOfAgeGroups      
            %for jjjj = 1:pa.AccessibilityGroupNumbers    
        
                Equations(end+1,:) = {['(param BetaVAge',num2str(kkk),num2str(k),' ',num2str(param.betaV(kkk,k)),' ) ']};
            %end        
        end      
    end   
%end
Equations(end+1,:) = {['']};

for j = 1:numberOfNodes
    for jj = 1:pa.AccessibilityGroupNumbers
        Equations(end+1,:) = {['(param Node',num2str(j),...
        'AccessGroupPercent',num2str(jj),' ',num2str(param.accessGroupPercent(j,jj)),')']};
    end
end

Equations(end+1,:) = {['']};
for kkk = 1:pa.InfectedWildComp
    Equations(end+1,:) = {['(param TransitionW',num2str(kkk),' ',num2str(-param.DTMatrix(kkk,kkk)),')']};
end

for kkk = 1:pa.InfectedWildComp
    Equations(end+1,:) = {['(param TransitionV',num2str(kkk),' ',num2str(-param.DTMatrix(kkk,kkk)),')']};
end


Equations(end+1,:) = {['']};
Equations(end+1,:) = {['; infection rates']};

for jj = 1:numberOfNodes
    %for jjj = 1:pa.AccessibilityGroupNumbers
        for kkk = 1:pa.numberOfAgeGroups   
        tempstr = {['']};
            if (pa.numberOfAgeGroups == 1) && (pa.AccessibilityGroupNumbers == 1) && (pa.InfectedWildComp == 1)
                tempstr = strcat(tempstr,['(func Node',num2str(jj),'LambdaWAge',num2str(kkk),' ']);
            elseif (pa.numberOfAgeGroups == 1) && (pa.AccessibilityGroupNumbers == 1) && (pa.InfectedWildComp > 1)
                tempstr = strcat(tempstr,['(func Node',num2str(jj),'LambdaWAge',num2str(kkk),' ']);
            else
                tempstr = strcat(tempstr,['(func Node',num2str(jj),'LambdaWAge',num2str(kkk),' (+']);
            end
            
            for k = 1:pa.numberOfAgeGroups
                %for jjjj = 1:pa.AccessibilityGroupNumbers    
                    tempstr = strcat(tempstr,[' (* BetaWAge',num2str(kkk),num2str(k),' ']);
                    for j = 1:pa.InfectedWildComp
                        if pa.InfectedWildComp > 1
                           
                            if pa.WhichCompAreInfectious(j) == 1
                                if j == pa.InfectedWildComp
                                    tempstr = strcat(tempstr,[' Node',num2str(jj),'::W',num2str(j),'Age',num2str(k),'))']);
                                elseif (pa.WhichCompAreInfectious(j) == 1) && (pa.WhichCompAreInfectious(j-1) == 0)
                                    tempstr = strcat(tempstr,[' (+ Node',num2str(jj),'::W',num2str(j),'Age',num2str(k),'']);
                                else 
                                    tempstr = strcat(tempstr,[' Node',num2str(jj),'::W',num2str(j),'Age',num2str(k),'']);
                                end
                            end
                        else
                            tempstr = strcat(tempstr,[' Node',num2str(jj),'::W',num2str(j),'Age',num2str(k),')']);
                        end
                    end
                %end
            end
            
            if (pa.numberOfAgeGroups > 1) || (pa.AccessibilityGroupNumbers > 1)
            tempstr = strcat(tempstr,')');
            end
            Equations(end+1,:) = strcat(tempstr,')');
        end
    %end
end
Equations(end+1,:) = {['']};

for jj = 1:numberOfNodes
    %for jjj = 1:pa.AccessibilityGroupNumbers
        for kkk = 1:pa.numberOfAgeGroups   
        tempstr = {['']};
        if (pa.numberOfAgeGroups == 1) && (pa.AccessibilityGroupNumbers == 1) && (pa.InfectedWildComp == 1)
            tempstr = strcat(tempstr,['(func Node',num2str(jj),'LambdaVAge',num2str(kkk),' ']);
        elseif (pa.numberOfAgeGroups == 1) && (pa.AccessibilityGroupNumbers == 1) && (pa.InfectedWildComp > 1)
            tempstr = strcat(tempstr,['(func Node',num2str(jj),'LambdaVAge',num2str(kkk),' ']);
        else
            tempstr = strcat(tempstr,['(func Node',num2str(jj),'LambdaVAge',num2str(kkk),' (+']);
        end
    
        for k = 1:pa.numberOfAgeGroups
            %for jjjj = 1:pa.AccessibilityGroupNumbers    
                tempstr = strcat(tempstr,[' (* BetaVAge',num2str(kkk),num2str(k),' ']);
                for j = 1:pa.InfectedWildComp
        
                    if pa.InfectedWildComp > 1
                        
                        if pa.WhichCompAreInfectious(j) == 1
                            if j == pa.InfectedWildComp
                                tempstr = strcat(tempstr,[' Node',num2str(jj),'::V',num2str(j),'Age',num2str(k),'))']);
                            elseif (pa.WhichCompAreInfectious(j) == 1) && (pa.WhichCompAreInfectious(j-1) == 0)
                                tempstr = strcat(tempstr,[' (+ Node',num2str(jj),'::V',num2str(j),'Age',num2str(k),'']);
                            else
                                tempstr = strcat(tempstr,[' Node',num2str(jj),'::V',num2str(j),'Age',num2str(k),'']);
                            end
                        end    
                     else
                        tempstr = strcat(tempstr,[' Node',num2str(jj),'::V',num2str(j),'Age',num2str(k),')']);
                    end
                end
            %end
        end
        
        if (pa.numberOfAgeGroups > 1) || (pa.AccessibilityGroupNumbers > 1)
            tempstr = strcat(tempstr,')');
        end
        Equations(end+1,:) = strcat(tempstr,')');
        end
    %end
end

Equations(end+1,:) = {['']};

for j = 1:numberOfNodes
    Equations(end+1,:) = {[';%%%%%%%%%%%']};
    Equations(end+1,:) = {[';Node ',num2str(j),' SIA parameters']};
    Equations(end+1,:) = {[';%%%%%%%%%%%']};
    if pa.numOfSIAsPerNode(j) > 0
    
        for jj = 1:pa.numOfSIAsPerNode(j)
            for jjj = 1:pa.AccessibilityGroupNumbers
                for jjjj = 1:pa.numberOfAgeGroups
                    Equations(end+1,:) = {['(param SIA',num2str(jj),'MagNode',num2str(j),'Access',num2str(jjj),'Age',num2str(jjjj),' 0)']};      
                end
            end
        end
    end
end

%          Equations(end+1,:) = {['(observe SIA1MagNode1Access1Age1 SIA1MagNode1Access1Age1)']};
%          Equations(end+1,:) = {['(observe SIA1MagNode1Access2Age1 SIA1MagNode1Access2Age1)']};
%          Equations(end+1,:) = {['(observe SIA1MagNode1Access3Age1 SIA1MagNode1Access3Age1)']};
%          Equations(end+1,:) = {['(observe SIA1MagNode1Access4Age1 SIA1MagNode1Access4Age1)']};

for jj = 1:numberOfNodes
    
% S 
    Equations(end+1,:) = {['']};
Equations(end+1,:) = {[';%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%']};    
Equations(end+1,:) = {[';%%%%%  Node ',num2str(jj),' Reactions   %%%%%']};   
Equations(end+1,:) = {[';%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%']};   
    
    
for j = 1:pa.AccessibilityGroupNumbers
    
    for k = 1:pa.numberOfAgeGroups
        Equations(end+1,:) = {['']};
        Equations(end+1,:) = {[';%%%%% Node',num2str(jj),'S',num2str(j),'Age',num2str(k),' dot %%%%%%']};
        Equations(end+1,:) = {['']};

        Equations(end+1,:) = {['(reaction infectionNode',num2str(jj),'VS',num2str(1),'Age',num2str(k),' (Node',num2str(jj),'::S',num2str(j),...
            'Age',num2str(k),') (Node',num2str(jj),'::V',num2str(1),'Age',num2str(k),') (* Node',num2str(jj),'::S',num2str(j),'Age',num2str(k), ...
            ' (/ Node',num2str(jj),'LambdaVAge',num2str(k),' Node',num2str(jj),'N)))']};


        Equations(end+1,:) = {['(reaction infectionNode',num2str(jj),'WS',num2str(1),'Age',num2str(k),' (Node',num2str(jj),'::S',num2str(j),...
            'Age',num2str(k),') (Node',num2str(jj),'::W',num2str(1),'Age',num2str(k),') (* Node',num2str(jj),'::S',num2str(j),'Age',num2str(k), ...
            ' (/ Node',num2str(jj),'LambdaWAge',num2str(k),' Node',num2str(jj),'N)))']};

        Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'S',num2str(j),'Age',num2str(k),' (Node',num2str(jj),'::S',num2str(j),'Age',num2str(k), ...
            ') () (* muAge',num2str(k),' Node',num2str(jj),'::S',num2str(j),'Age',num2str(k),'))']};

        if k == 1
            Equations(end+1,:) = {['(reaction BirthNode',num2str(jj),'S',num2str(j),'Age',num2str(1),' () (Node',num2str(jj),'::S',num2str(j),...
            'Age',num2str(1),')  (* Node',num2str(jj),'nu (* Node',num2str(jj),'N (* (- 1 (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacy ', ...
            num2str(pa.DropOffRateRoutinePerAgeGroup(k)),')) Node',num2str(jj),...
            'AccessGroupPercent',num2str(j),'))))']};

             Equations(end+1,:) = {['(reaction BirthNode',num2str(jj),'V',num2str(j),'Age',num2str(1),' () (Node',num2str(jj),'::V',num2str(1),...
            'Age',num2str(1),')  (* Node',num2str(jj),'nu (* Node',num2str(jj),'N (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacy ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),' Node',num2str(jj),...
            'AccessGroupPercent',num2str(j),'))))']};

            if pa.numberOfAgeGroups > 1
                
                if pa.DropOffRateRoutinePerAgeGroup(k+1) == 0
                    Equations(end+1,:) = {['(reaction AgingS',num2str(j),'Age',num2str(k),'Node',num2str(jj),' (Node',num2str(jj),'::S',num2str(j),...
                    'Age',num2str(k),') (Node',num2str(jj),'::S',num2str(j),...
                    'Age',num2str(k+1),')  (* alphaAge',num2str(k),' Node',num2str(jj),'::S',num2str(j),...
                    'Age',num2str(k),'))']};
                else
                    Equations(end+1,:) = {['(reaction AgingS',num2str(j),'Age',num2str(k),'Node',num2str(jj),' (Node',num2str(jj),'::S',num2str(j),...
                    'Age',num2str(k),') (Node',num2str(jj),'::S',num2str(j),...
                    'Age',num2str(k+1),')  (* (- 1 (* VaccineEfficacy Node',num2str(jj),'gRoutAccess',num2str(j),' ',num2str(pa.DropOffRateRoutinePerAgeGroup(k+1)),...
                    ')) alphaAge',num2str(k),' Node',num2str(jj),'::S',num2str(j),...
                    'Age',num2str(k),'))']};     
                
                     Equations(end+1,:) = {['(reaction AgingRoutS',num2str(j),'Age',num2str(k),'Node',num2str(jj),' (Node',num2str(jj),'::S',num2str(j),...
                    'Age',num2str(k),') (Node',num2str(jj),'::V1Age',num2str(k+1), ...
                    ')  (* VaccineEfficacy Node',num2str(jj),'gRoutAccess',num2str(j),' ',num2str(pa.DropOffRateRoutinePerAgeGroup(k+1)),...
                    ' alphaAge',num2str(k),' Node',num2str(jj),'::S',num2str(j),...
                    'Age',num2str(k),'))']}; 
                end
            
                
            
            end
    
        elseif k~= pa.numberOfAgeGroups
            if pa.numberOfAgeGroups > 1
                if pa.DropOffRateRoutinePerAgeGroup(k+1) == 0
                    Equations(end+1,:) = {['(reaction AgingS',num2str(j),'Age',num2str(k),'Node',num2str(jj),' (Node',num2str(jj),'::S',num2str(j),...
                    'Age',num2str(k),') (Node',num2str(jj),'::S',num2str(j),...
                    'Age',num2str(k+1),')  (* alphaAge',num2str(k),' Node',num2str(jj),'::S',num2str(j),...
                    'Age',num2str(k),'))']};
                else
                    Equations(end+1,:) = {['(reaction AgingS',num2str(j),'Age',num2str(k),'Node',num2str(jj),' (Node',num2str(jj),'::S',num2str(j),...
                    'Age',num2str(k),') (Node',num2str(jj),'::S',num2str(j),...
                    'Age',num2str(k+1),')  (* (- 1 (* VaccineEfficacy Node',num2str(jj),'gRoutAccess',num2str(j),' ',num2str(pa.DropOffRateRoutinePerAgeGroup(k+1)),...
                    ')) alphaAge',num2str(k),' Node',num2str(jj),'::S',num2str(j),...
                    'Age',num2str(k),'))']};     
                
                     Equations(end+1,:) = {['(reaction AgingRoutS',num2str(j),'Age',num2str(k),'Node',num2str(jj),' (Node',num2str(jj),'::S',num2str(j),...
                    'Age',num2str(k),') (Node',num2str(jj),'::V1Age',num2str(k+1), ...
                    ')  (* VaccineEfficacy Node',num2str(jj),'gRoutAccess',num2str(j),' ',num2str(pa.DropOffRateRoutinePerAgeGroup(k+1)),...
                    ' alphaAge',num2str(k),' Node',num2str(jj),'::S',num2str(j),...
                    'Age',num2str(k),'))']}; 
                end
            end
        else
            Equations(end+1,:) = {['(reaction AgingS',num2str(j),'Age',num2str(k),'Node',num2str(jj),' (Node',num2str(jj),'::S',num2str(j),...
            'Age',num2str(k),') ()  (* alphaAge',num2str(k),' Node',num2str(jj),'::S',num2str(j),...
            'Age',num2str(k),'))']};
        end    
    end
end

%% SIAS
    Equations(end+1,:) = {['']};
    Equations(end+1,:) = {[';%%%%%% Node ',num2str(jj),' SIAs %%%%%%']};
    Equations(end+1,:) = {['']};
    
if pa.numOfSIAsPerNode(jj) > 0
    for jjjj = 1:pa.numOfSIAsPerNode(jj)
        for jjj = 1:pa.AccessibilityGroupNumbers
            for jjjjj = 1:pa.numberOfAgeGroups
                Equations(end+1,:) = {['(reaction Node',num2str(jj),'Impulse',num2str(jjjj),' (Node',num2str(jj),'::S',num2str(jjj),...
                'Age',num2str(jjjjj),') (Node',num2str(jj),'::V1',...
                'Age',num2str(jjjjj),') SIA',num2str(jjjj),'MagNode',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjjj),')']};
            end
        end
    end
end

    Equations(end+1,:) = {['']};
    Equations(end+1,:) = {[';%%%%%% Node ',num2str(jj),' SIAs Time Dependent Reactions %%%%%%']};
    Equations(end+1,:) = {['']};
 
    if pa.numOfSIAsPerNode(jj) > 0
        for jjjjj = 1:pa.numOfSIAsPerNode(jj)
            for jjj = 1:pa.AccessibilityGroupNumbers
                for jjjj = 1:pa.numberOfAgeGroups
                    
                    if strcmp(pa.TypeOfSIACampaign,'Periodic')
                        eval(['Time = pa.PeriodicCampaignTimingVec.Node',num2str(jj),'(jjjjj);']);
                        eval(['GlobalCoverage = pa.GlobalSIACoverageVec.Node',num2str(jj),'(jjjjj);']); 
                    else
                        eval(['Time = pa.NonPeriodicCampaignTiming.Node',num2str(jj),'(jjjjj);']);
                    end
                       
                    Cov = ComputeGCCCoverage(pa,jj,jjjjj);
                    eval(['CC = pa.GlobalSIACoverageVaccEffVec.Node',num2str(jj),'(jjjjj) * Cov(jjj)/pa.SIADuration;']);
                    
                    Equations(end+1,:) = {['(time-event ImpulsivePert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((SIA',num2str(jjjjj),'MagNode',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' (* Node',num2str(jj),'::S',num2str(jjj),'Age',num2str(jjjj),' ',num2str(CC),'))))']};
                    Equations(end+1,:) = {['(time-event ImpulsivePert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj), ...
                        ' ',num2str(Time+pa.SIADuration), ...
                        ' ((SIA',num2str(jjjjj),'MagNode',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),' 0)))']};
%     
%                     Equations(end+1,:) = {['']};
                                        
                end
            end
        end
    end

    %Need to edit this section
%% W Stuff
    
%for j = 1:pa.AccessibilityGroupNumbers
    for k = 1:pa.numberOfAgeGroups
        
        Equations(end+1,:) = {['']};
        Equations(end+1,:) = {['; %%%%% Node',num2str(jj),'WAge',num2str(k),' dot %%%%%% ']};
        Equations(end+1,:) = {['']};
        for kkk = 1:pa.InfectedWildComp

            Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'W',num2str(kkk),'Age',num2str(k),' (Node',num2str(jj),'::W',num2str(kkk),'Age',num2str(k), ...
            ') () (* muAge',num2str(k),' Node',num2str(jj),'::W',num2str(kkk),'Age',num2str(k), ...
            '))']};

            if kkk ~= pa.InfectedWildComp
    
                Equations(end+1,:) = {['(reaction WTransitionNode',num2str(jj),'W',num2str(kkk),'Age',num2str(k),' (Node',num2str(jj),'::W',num2str(kkk),'Age',num2str(k), ...
                ') (Node',num2str(jj),'::W',num2str(kkk+1),'Age',num2str(k), ...
                ') (* TransitionW',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Age',num2str(k), ...
                '))']};   
            else
    
                Equations(end+1,:) = {['(reaction WRecoveryNode',num2str(jj),'W',num2str(kkk),'Age',num2str(k),' (Node',num2str(jj),'::W',num2str(kkk),'Age',num2str(k), ...
                ') (Node',num2str(jj),'::RAge',num2str(k), ...
                ') (* TransitionW',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Age',num2str(k), ...
                '))']};  
            end

            if pa.numberOfAgeGroups > 1
                if k ~= pa.numberOfAgeGroups
                    Equations(end+1,:) = {['(reaction AgingW',num2str(kkk),'Age',num2str(k),'Node',num2str(jj),' (Node',num2str(jj),'::W',num2str(kkk),...
                    'Age',num2str(k),') (Node',num2str(jj),'::W',num2str(kkk),...
                    'Age',num2str(k+1),')  (* alphaAge',num2str(k),' Node',num2str(jj),'::W',num2str(kkk),...
                    'Age',num2str(k),' ) )']};
                else
                    Equations(end+1,:) = {['(reaction AgingW',num2str(kkk),'Age',num2str(k),'Node',num2str(jj),' (Node',num2str(jj),'::W',num2str(kkk),...
                    'Age',num2str(k),') ()  (* alphaAge',num2str(k),' Node',num2str(jj),'::W',num2str(kkk),...
                    'Age',num2str(k),' ) )']};
                end
            end
            Equations(end+1,:) = {['']};
        end
    end
%end


    %% V Stuff
for j = 1:pa.AccessibilityGroupNumbers
    
    for k = 1:pa.numberOfAgeGroups
        
Equations(end+1,:) = {['']};
Equations(end+1,:) = {[';%%%%% Node',num2str(jj),'VAge',num2str(k),' dot %%%%% ']};
        Equations(end+1,:) = {['']};
for kkk = 1:pa.InfectedVaccComp

Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'V',num2str(kkk),'Age',num2str(k),' (Node',num2str(jj),'::V',num2str(kkk),'Age',num2str(k), ...
    ') () (* muAge',num2str(k),' Node',num2str(jj),'::V',num2str(kkk),'Age',num2str(k), ...
    '))']};

if kkk ~= pa.InfectedVaccComp
    
    Equations(end+1,:) = {['(reaction VTransitionNode',num2str(jj),'V',num2str(kkk),'Age',num2str(k),' (Node',num2str(jj),'::V',num2str(kkk),'Age',num2str(k), ...
    ') (Node',num2str(jj),'::V',num2str(kkk+1),'Age',num2str(k), ...
    ') (* TransitionW',num2str(kkk),' Node',num2str(jj),'::V',num2str(kkk),'Age',num2str(k), ...
    '))']};
       
else
    
    Equations(end+1,:) = {['(reaction VRecoveryNode',num2str(jj),'V',num2str(kkk),'Age',num2str(k),' (Node',num2str(jj),'::V',num2str(kkk),'Age',num2str(k), ...
    ') (Node',num2str(jj),'::RAge',num2str(k), ...
    ') (* TransitionW',num2str(kkk),' Node',num2str(jj),'::V',num2str(kkk),'Age',num2str(k), ...
    '))']};
end
if pa.numberOfAgeGroups > 1
if k ~= pa.numberOfAgeGroups

    Equations(end+1,:) = {['(reaction AgingV',num2str(kkk),'Age',num2str(k),'Node',num2str(jj),' (Node',num2str(jj),'::V',num2str(kkk),...
    'Age',num2str(k),') (Node',num2str(jj),'::V',num2str(kkk),...
    'Age',num2str(k+1),')  (* alphaAge',num2str(k),' Node',num2str(jj),'::V',num2str(kkk),...
    'Age',num2str(k),' ) )']};
  
else

    Equations(end+1,:) = {['(reaction AgingV',num2str(kkk),'Age',num2str(k),'Node',num2str(jj),' (Node',num2str(jj),'::V',num2str(kkk),...
    'Age',num2str(k),') ()  (* alphaAge',num2str(k),' Node',num2str(jj),'::V',num2str(kkk),...
    'Age',num2str(k),' ) )']};
    
end
end
Equations(end+1,:) = {['']};

        end
    end
    
end


%% R Stuff 
for k = 1:pa.numberOfAgeGroups
        
Equations(end+1,:) = {['']};
Equations(end+1,:) = {[';%%%%% Node',num2str(jj),'RAge',num2str(k),' dot %%%%%']};
        Equations(end+1,:) = {['']};

Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'RAge',num2str(k),' (Node',num2str(jj),'::RAge',num2str(k), ...
    ') () (* muAge',num2str(k),' Node',num2str(jj),'::RAge',num2str(k), ...
    '))']};
if pa.numberOfAgeGroups > 1
if k ~= pa.numberOfAgeGroups

    Equations(end+1,:) = {['(reaction AgingRAge',num2str(k),'Node',num2str(jj),'Access (Node',num2str(jj),'::RAge'...
        ,num2str(k),') (Node',num2str(jj),'::RAge',...
        num2str(k+1),')  (* alphaAge',num2str(k),' Node',num2str(jj),'::RAge',num2str(k),' ) )']};
  
else
   Equations(end+1,:) = {['(reaction AgingRAge',num2str(k),'Node',num2str(jj),'Access (Node',num2str(jj),'::RAge'...
        ,num2str(k),') ()  (* alphaAge',num2str(k),' Node',num2str(jj),'::RAge',num2str(k),' ) )']};
    
end
end
    
end


end

%% Migration
Equations(end+1,:) = {['']};
Equations(end+1,:) = {['; %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%']};
Equations(end+1,:) = {['; %%%% Migration among nodes %%%%']};
Equations(end+1,:) = {['; %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%']};
Equations(end+1,:) = {['']};
for j = 1 : numberOfNodes
    

Equations(end+1,:) = {['; %%%% Migration Node ',num2str(j),' Away %%%%']};

Equations(end+1,:) = {['']};
    
   for jj = 1:numberOfNodes
    
    if pa.Mig(j,jj) ~= 0
    
        GeneralMigRate = pa.Mig(j,jj);
        m = 1;
for jjj = 1:pa.AccessibilityGroupNumbers
    for k = 1:pa.numberOfAgeGroups

        Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'S',num2str(jjj),'Age',num2str(k), ...
            ' (Node',num2str(j),'::S',num2str(jjj),'Age',num2str(k),') (Node',num2str(jj),'::S',num2str(jjj),'Age',num2str(k),...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::S',num2str(jjj),'Age',num2str(k),'))']};
        m = m+1;
    end
end

%for jjj = 1:pa.AccessibilityGroupNumbers
for jjjj = 1:pa.InfectedWildComp
    for k = 1:pa.numberOfAgeGroups
        
        Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'W',num2str(jjjj),'Age',num2str(k), ...
            ' (Node',num2str(j),'::W',num2str(jjjj),'Age',num2str(k),') (Node',num2str(jj),'::W',num2str(jjjj),'Age',num2str(k),...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::W',num2str(jjjj),'Age',num2str(k),'))']};
        m = m+1;
        
    end
end
%end

%for jjj = 1:pa.AccessibilityGroupNumbers
for jjjj = 1:pa.InfectedVaccComp
     for k = 1:pa.numberOfAgeGroups

             Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'V',num2str(jjjj),'Age',num2str(k), ...
            ' (Node',num2str(j),'::V',num2str(jjjj),'Age',num2str(k),') (Node',num2str(jj),'::V',num2str(jjjj),'Age',num2str(k),...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::V',num2str(jjjj),'Age',num2str(k),'))']};
        m = m+1;
        
     end
end
%end

for k = 1:pa.numberOfAgeGroups

            Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'RAge',num2str(k), ...
            ' (Node',num2str(j),'::RAge',num2str(k),') (Node',num2str(jj),'::RAge',num2str(k),...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::RAge',num2str(k),'))']};
        m = m+1;
    
end   
    end 
   end
   Equations(end+1,:) = {['']};
end


Equations(end+1,:) = {['(end-model)']};


% open the file with write permission
fid = fopen('modelfile.emodl', 'w');
fprintf(fid, '%s \n', Equations{:});
fclose(fid);

end