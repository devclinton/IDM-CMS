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

    for jjj = 1:pa.HowManyWildTypes
        for j = 1:pa.InfectedWildComp
            for k = 1:pa.numberOfAgeGroups
                
                if jjj == 1
                Equations(end+1,:) = {['(species Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN ',num2str(IC(m)),')']};  m = m+1;
                Equations(end+1,:) = {['(species Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune2 ',num2str(IC(m)),')']};  m = m+1;
                Equations(end+1,:) = {['(species Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune3 ',num2str(IC(m)),')']};  m = m+1;
                Equations(end+1,:) = {['(species Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune23 ',num2str(IC(m)),')']};
                m = m+1;
                
                elseif jjj == 2
                Equations(end+1,:) = {['(species Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN ',num2str(IC(m)),')']};  m = m+1;    
                Equations(end+1,:) = {['(species Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1 ',num2str(IC(m)),')']};  m = m+1;
                Equations(end+1,:) = {['(species Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune3 ',num2str(IC(m)),')']};  m = m+1;
                Equations(end+1,:) = {['(species Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune13 ',num2str(IC(m)),')']};
                m = m+1;
                    
                else
                Equations(end+1,:) = {['(species Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN ',num2str(IC(m)),')']};  m = m+1;   
                Equations(end+1,:) = {['(species Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1 ',num2str(IC(m)),')']};  m = m+1;
                Equations(end+1,:) = {['(species Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune2 ',num2str(IC(m)),')']};  m = m+1;
                Equations(end+1,:) = {['(species Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune12 ',num2str(IC(m)),')']};
                m = m+1;
                    
                end
            end
        end
    end

    %for jjj = 1:pa.AccessibilityGroupNumbers
    if pa.SIADuration > 0
        for j = 1:pa.InfectedVaccComp
            for k = 1:pa.numberOfAgeGroups
                Equations(end+1,:) = {['(species Node',num2str(jj),'::V',num2str(j),'Age',num2str(k),' ',num2str(IC(m)),')']};
                m = m+1;
            end
        end
    end
    %end

    for k = 1:pa.numberOfAgeGroups
        Equations(end+1,:) = {['(species Node',num2str(jj),'::RImmune1Age',num2str(k),' ',num2str(IC(m)),')']};
        Equations(end+1,:) = {['(species Node',num2str(jj),'::RImmune2Age',num2str(k),' ',num2str(IC(m)),')']};
        Equations(end+1,:) = {['(species Node',num2str(jj),'::RImmune3Age',num2str(k),' ',num2str(IC(m)),')']};
        
        Equations(end+1,:) = {['(species Node',num2str(jj),'::RImmune12Age',num2str(k),' ',num2str(IC(m)),')']};
        Equations(end+1,:) = {['(species Node',num2str(jj),'::RImmune23Age',num2str(k),' ',num2str(IC(m)),')']};
        Equations(end+1,:) = {['(species Node',num2str(jj),'::RImmune13Age',num2str(k),' ',num2str(IC(m)),')']};
        
        Equations(end+1,:) = {['(species Node',num2str(jj),'::RImmune123Age',num2str(k),' ',num2str(IC(m)),')']};
        
        
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

    for jjj = 1:pa.HowManyWildTypes
        for j = 1:pa.InfectedWildComp
            for k = 1:pa.numberOfAgeGroups
                
                if jjj == 1
                    Equations(end+1,:) = {['(observe InfectiousWildNode',num2str(jj),'W',num2str(j),'Type',num2str(jjj),'Age',num2str(k), ...
                        'ImmuneN Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)']};
                    Equations(end+1,:) = {['(observe InfectiousWildNode',num2str(jj),'W',num2str(j),'Type',num2str(jjj),'Age',num2str(k), ...
                        'Immune2 Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune2)']};
                    Equations(end+1,:) = {['(observe InfectiousWildNode',num2str(jj),'W',num2str(j),'Type',num2str(jjj),'Age',num2str(k), ...
                        'Immune3 Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune3)']};
                    Equations(end+1,:) = {['(observe InfectiousWildNode',num2str(jj),'W',num2str(j),'Type',num2str(jjj),'Age',num2str(k), ...
                        'Immune23 Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune23)']};
                elseif jjj == 2
                    Equations(end+1,:) = {['(observe InfectiousWildNode',num2str(jj),'W',num2str(j),'Type',num2str(jjj),'Age',num2str(k), ...
                        'ImmuneN Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)']};
                    Equations(end+1,:) = {['(observe InfectiousWildNode',num2str(jj),'W',num2str(j),'Type',num2str(jjj),'Age',num2str(k), ...
                        'Immune1 Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1)']};
                    Equations(end+1,:) = {['(observe InfectiousWildNode',num2str(jj),'W',num2str(j),'Type',num2str(jjj),'Age',num2str(k), ...
                        'Immune3 Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune3)']};
                    Equations(end+1,:) = {['(observe InfectiousWildNode',num2str(jj),'W',num2str(j),'Type',num2str(jjj),'Age',num2str(k), ...
                        'Immune13 Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune13)']};
                else
                    Equations(end+1,:) = {['(observe InfectiousWildNode',num2str(jj),'W',num2str(j),'Type',num2str(jjj),'Age',num2str(k), ...
                        'ImmuneN Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)']};
                    Equations(end+1,:) = {['(observe InfectiousWildNode',num2str(jj),'W',num2str(j),'Type',num2str(jjj),'Age',num2str(k), ...
                        'Immune1 Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1)']};
                    Equations(end+1,:) = {['(observe InfectiousWildNode',num2str(jj),'W',num2str(j),'Type',num2str(jjj),'Age',num2str(k), ...
                        'Immune2 Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune2)']};
                    Equations(end+1,:) = {['(observe InfectiousWildNode',num2str(jj),'W',num2str(j),'Type',num2str(jjj),'Age',num2str(k), ...
                        'Immune12 Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune12)']};
                end
            end
        end
    end

    %for jjj = 1:pa.AccessibilityGroupNumbers
    if pa.SIADuration > 0
        for j = 1:pa.InfectedVaccComp
            for k = 1:pa.numberOfAgeGroups
                Equations(end+1,:) = {['(observe InfectiousVaccNode',num2str(jj),'V',num2str(j),'Age',num2str(k), ...
                ' Node',num2str(jj),'::V',num2str(j),'Age',num2str(k),')']};
            end
        end
    end
    %end


    for k = 1:pa.numberOfAgeGroups
        %Equations(end+1,:) = {['(observe RNode',num2str(jj),'RAge',num2str(k), ...
        %' Node',num2str(jj),'::RAge',num2str(k),')']};
    
        Equations(end+1,:) = {['(observe RecNode',num2str(jj),'::RImmune1Age',num2str(k),' Node',num2str(jj),'::RImmune1Age',num2str(k),')']};
        Equations(end+1,:) = {['(observe RecNode',num2str(jj),'::RImmune2Age',num2str(k),' Node',num2str(jj),'::RImmune2Age',num2str(k),')']};
        Equations(end+1,:) = {['(observe RecNode',num2str(jj),'::RImmune3Age',num2str(k),' Node',num2str(jj),'::RImmune3Age',num2str(k),')']};
        
        Equations(end+1,:) = {['(observe RecNode',num2str(jj),'::RImmune12Age',num2str(k),' Node',num2str(jj),'::RImmune12Age',num2str(k),')']};
        Equations(end+1,:) = {['(observe RecNode',num2str(jj),'::RImmune23Age',num2str(k),' Node',num2str(jj),'::RImmune23Age',num2str(k),')']};
        Equations(end+1,:) = {['(observe RecNode',num2str(jj),'::RImmune13Age',num2str(k),' Node',num2str(jj),'::RImmune13Age',num2str(k),')']};
        
        Equations(end+1,:) = {['(observe RecNode',num2str(jj),'::RImmune123Age',num2str(k),' Node',num2str(jj),'::RImmune123Age',num2str(k),')']}; 
    
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
Equations(end+1,:) = {['(param VaccineEfficacyTOPVType1 ',num2str(pa.VaccineEfficacyTOPVType1),')']};
Equations(end+1,:) = {['(param VaccineEfficacyTOPVType2 ',num2str(pa.VaccineEfficacyTOPVType2),')']};
Equations(end+1,:) = {['(param VaccineEfficacyTOPVType3 ',num2str(pa.VaccineEfficacyTOPVType3),')']};
Equations(end+1,:) = {['(param VaccineEfficacyBOPVType1 ',num2str(pa.VaccineEfficacyBOPVType1),')']};
Equations(end+1,:) = {['(param VaccineEfficacyBOPVType3 ',num2str(pa.VaccineEfficacyBOPVType3),')']};
Equations(end+1,:) = {['(param VaccineEfficacyMOPVType1 ',num2str(pa.VaccineEfficacyMOPVType3),')']};
Equations(end+1,:) = {['(param VaccineEfficacyMOPVType3 ',num2str(pa.VaccineEfficacyMOPVType3),')']};
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
for kkkk = 1 : pa.HowManyWildTypes
    for kkk = 1:pa.numberOfAgeGroups    
        for k = 1:pa.numberOfAgeGroups      
            %for jjjj = 1:pa.AccessibilityGroupNumbers    
                eval(['temp = param.betaWType',num2str(kkkk),'(kkk,k);']);
                Equations(end+1,:) = {['(param BetaWType',num2str(kkkk),'Age',num2str(kkk),num2str(k),' ',num2str(temp),' ) ']};
            %end        
        end      
    end   
end
%end
Equations(end+1,:) = {['']};

if pa.SIADuration > 0
%for jjj = 1:pa.AccessibilityGroupNumbers
    for kkk = 1:pa.numberOfAgeGroups    
        for k = 1:pa.numberOfAgeGroups      
            %for jjjj = 1:pa.AccessibilityGroupNumbers    
        
                Equations(end+1,:) = {['(param BetaVAge',num2str(kkk),num2str(k),' ',num2str(param.betaV(kkk,k)),' ) ']};
            %end        
        end      
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
for kkkk = 1:pa.HowManyWildTypes
    for kkk = 1:pa.InfectedWildComp
        Equations(end+1,:) = {['(param TransitionType',num2str(kkkk),'W',num2str(kkk),' ',num2str(-param.DTMatrix(kkk,kkk)),')']};
    end
end
if pa.SIADuration > 0 
    for kkk = 1:pa.InfectedWildComp
        Equations(end+1,:) = {['(param TransitionV',num2str(kkk),' ',num2str(-param.DTMatrix(kkk,kkk)),')']};
    end
end

Equations(end+1,:) = {['']};
Equations(end+1,:) = {['; infection rates']};


for jj = 1:numberOfNodes
    for jjj = 1:pa.HowManyWildTypes
        for kkk = 1:pa.numberOfAgeGroups   
        tempstr = {['']};
            if (pa.numberOfAgeGroups == 1) && (pa.AccessibilityGroupNumbers == 1) && (pa.InfectedWildComp == 1)
                tempstr = strcat(tempstr,['(func Node',num2str(jj),'LambdaWType',num2str(jjj),'Age',num2str(kkk),' ']);
            elseif (pa.numberOfAgeGroups == 1) && (pa.AccessibilityGroupNumbers == 1) && (pa.InfectedWildComp > 1)
                tempstr = strcat(tempstr,['(func Node',num2str(jj),'LambdaWType',num2str(jjj),'Age',num2str(kkk),' ']);
            else
                tempstr = strcat(tempstr,['(func Node',num2str(jj),'LambdaWType',num2str(jjj),'Age',num2str(kkk),' (+']);
            end
            
            for k = 1:pa.numberOfAgeGroups
                %for jjjj = 1:pa.AccessibilityGroupNumbers    
                    tempstr = strcat(tempstr,[' (* BetaWType',num2str(jjj),'Age',num2str(kkk),num2str(k),' ']);
                    for j = 1:pa.InfectedWildComp
                        
                        if jjj == 1
                        
                        if (pa.InfectedWildComp > 1) 
                            
                            if pa.WhichCompAreInfectious(j) == 1
                                if (j == pa.InfectedWildComp) && (sum(pa.WhichCompAreInfectious) == 1)
                                    tempstr = strcat(tempstr,[' (+ Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune2'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune3'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune23'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)'...
                                        ')']);
                                elseif j == pa.InfectedWildComp
                                    tempstr = strcat(tempstr,[' (+ Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune2'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune3'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune23'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)'...
                                        '))']);
                                elseif (pa.WhichCompAreInfectious(j) == 1) && (pa.WhichCompAreInfectious(j-1) == 0)
                                    tempstr = strcat(tempstr,[' (+ (+ Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune2'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune3'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune23'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)'...
                                        ]);
                                else 
                                    tempstr = strcat(tempstr,[' (+ Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune2' ...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune3'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune23'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)'...
                                    ]);
                                end
                            end
                        else
                            tempstr = strcat(tempstr,[' (+ Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune2' ...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune3'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune23'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)'...
                            ')']);
                        end
                        
                        elseif jjj == 2
                            
                            if (pa.InfectedWildComp > 1) 
                            
                            if pa.WhichCompAreInfectious(j) == 1
                                if (j == pa.InfectedWildComp) && (sum(pa.WhichCompAreInfectious) == 1)
                                    tempstr = strcat(tempstr,[' (+ Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune13'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)'...
                                        ')']);
                                elseif j == pa.InfectedWildComp
                                    tempstr = strcat(tempstr,[' (+ Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune13'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)'...
                                        '))']);
                                elseif (pa.WhichCompAreInfectious(j) == 1) && (pa.WhichCompAreInfectious(j-1) == 0)
                                    tempstr = strcat(tempstr,[' (+ (+ Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune13'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)'...
                                        ]);
                                else 
                                    tempstr = strcat(tempstr,[' (+ Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1' ...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune13'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)'...
                                    ]);
                                end
                            end
                        else
                            tempstr = strcat(tempstr,[' (+ Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1' ...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune13'... 
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)'...
                                        ')']);
                        end
                            
                        else 
                            
                            if (pa.InfectedWildComp > 1) 
                            
                            if pa.WhichCompAreInfectious(j) == 1
                                if (j == pa.InfectedWildComp) && (sum(pa.WhichCompAreInfectious) == 1)
                                    tempstr = strcat(tempstr,[' (+ Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune2'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune12'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)'...
                                        ')']);
                                elseif j == pa.InfectedWildComp
                                    tempstr = strcat(tempstr,[' (+ Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune2'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune12'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)'...
                                        '))']);
                                elseif (pa.WhichCompAreInfectious(j) == 1) && (pa.WhichCompAreInfectious(j-1) == 0)
                                    tempstr = strcat(tempstr,[' (+ (+ Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune2'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune12'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)'...
                                        ]);
                                else 
                                    tempstr = strcat(tempstr,[' (+ Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1' ...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune2'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune12'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)'...
                                    ]);
                                end
                            end
                        else
                            tempstr = strcat(tempstr,[' (+ Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune1' ...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune2'...
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'Immune12'... 
                                        ' Node',num2str(jj),'::W',num2str(j),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN)'... 
                                        ')']);
                        end
                            
                            
                        end
                    end
                %end
            end
            
            if (pa.numberOfAgeGroups > 1) || (pa.AccessibilityGroupNumbers > 1)
            tempstr = strcat(tempstr,')');
            end
            Equations(end+1,:) = strcat(tempstr,')');
        end
    end
end
Equations(end+1,:) = {['']};

if pa.SIADuration > 0
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
                for j = 1:pa.InfectedVaccComp
        
                    if (pa.InfectedVaccComp > 1) 
                        
                        if pa.WhichCompAreInfectious(j) == 1
                            if (j == pa.InfectedVaccComp) && (sum(pa.WhichCompAreInfectious) == 1)
                                tempstr = strcat(tempstr,[' Node',num2str(jj),'::V',num2str(j),'Age',num2str(k),')']);
                            elseif (j == pa.InfectedVaccComp) 
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
end
Equations(end+1,:) = {['']};
if pa.SIADuration > 0
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
end

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

        if pa.SIADuration > 0
            Equations(end+1,:) = {['(reaction infectionNode',num2str(jj),'VS',num2str(1),'Age',num2str(k),' (Node',num2str(jj),'::S',num2str(j),...
            'Age',num2str(k),') (Node',num2str(jj),'::V',num2str(1),'Age',num2str(k),') (* Node',num2str(jj),'::S',num2str(j),'Age',num2str(k), ...
            ' (/ Node',num2str(jj),'LambdaVAge',num2str(k),' Node',num2str(jj),'N)))']};
        end
        
            Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'S',num2str(j),'Age',num2str(k),' (Node',num2str(jj),'::S',num2str(j),'Age',num2str(k), ...
            ') () (* muAge',num2str(k),' Node',num2str(jj),'::S',num2str(j),'Age',num2str(k),'))']};

        
        for jjj = 1:pa.HowManyWildTypes
            Equations(end+1,:) = {['(reaction infectionNode',num2str(jj),'WType',num2str(jjj),'S',num2str(1),'Age',num2str(k),' (Node',num2str(jj),'::S',num2str(j),...
            'Age',num2str(k),') (Node',num2str(jj),'::W',num2str(1),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN) (* Node',num2str(jj),'::S',num2str(j),'Age',num2str(k), ...
            ' (/ Node',num2str(jj),'LambdaWType',num2str(jjj),'Age',num2str(k),' Node',num2str(jj),'N)))']};
        end
        

        if k == 1
            Equations(end+1,:) = {['(reaction BirthNode',num2str(jj),'S',num2str(j),'Age',num2str(1),' () (Node',num2str(jj),'::S',num2str(j),...
            'Age',num2str(1),')  (* Node',num2str(jj),'nu (* Node',num2str(jj),'N (* ' ...
            '(- 1 (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType1 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),'))' ... 
            ' (- 1 (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType2 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),'))' ... 
            ' (- 1 (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType3 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),'))' ... 
            ' Node',num2str(jj),'AccessGroupPercent',num2str(j),'))))']};

%For Debug
%             Equations(end+1,:) = {['(reaction BirthNode',num2str(jj),'S',num2str(j),'Age',num2str(1),' () (Node',num2str(jj),'::S',num2str(j),...
%            'Age',num2str(1),') (* Node',num2str(jj),'nu Node',num2str(jj),'N))']};

             Equations(end+1,:) = {['(reaction BirthNode',num2str(jj),'R123Age',num2str(1),' () (Node',num2str(jj),'::RImmune123Age',num2str(k), ...
             ')  (* Node',num2str(jj),'nu (* Node',num2str(jj),'N (* ' ...
            '(* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType1 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),')' ... 
            ' (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType2 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),')' ... 
            ' (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType3 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),')' ... 
             ' Node',num2str(jj),'AccessGroupPercent',num2str(j),'))))']};
         
             Equations(end+1,:) = {['(reaction BirthNode',num2str(jj),'R12Age',num2str(1),' () (Node',num2str(jj),'::RImmune12Age',num2str(k), ...
             ')  (* Node',num2str(jj),'nu (* Node',num2str(jj),'N (* ' ...
            '(* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType1 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),')' ... 
            ' (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType2 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),')' ... 
            ' (- 1 (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType3 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),'))' ... 
             ' Node',num2str(jj),'AccessGroupPercent',num2str(j),'))))']};
         
             Equations(end+1,:) = {['(reaction BirthNode',num2str(jj),'R23Age',num2str(1),' () (Node',num2str(jj),'::RImmune23Age',num2str(k), ...
             ')  (* Node',num2str(jj),'nu (* Node',num2str(jj),'N (* ' ...
            '(- 1 (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType1 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),'))' ... 
            ' (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType2 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),')' ... 
            ' (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType3 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),')' ... 
             ' Node',num2str(jj),'AccessGroupPercent',num2str(j),'))))']};
         
             Equations(end+1,:) = {['(reaction BirthNode',num2str(jj),'R13Age',num2str(1),' () (Node',num2str(jj),'::RImmune13Age',num2str(k), ...
             ')  (* Node',num2str(jj),'nu (* Node',num2str(jj),'N (* ' ...
            '(* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType1 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),')' ... 
            ' (- 1 (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType2 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),'))' ... 
            ' (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType3 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),')' ... 
             ' Node',num2str(jj),'AccessGroupPercent',num2str(j),'))))']};
         
             Equations(end+1,:) = {['(reaction BirthNode',num2str(jj),'R1Age',num2str(1),' () (Node',num2str(jj),'::RImmune1Age',num2str(k), ...
             ')  (* Node',num2str(jj),'nu (* Node',num2str(jj),'N (* ' ...
            '(* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType1 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),')' ... 
            ' (- 1 (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType2 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),'))' ... 
            ' (- 1 (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType3 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),'))' ... 
             ' Node',num2str(jj),'AccessGroupPercent',num2str(j),'))))']};
         
             Equations(end+1,:) = {['(reaction BirthNode',num2str(jj),'R2Age',num2str(1),' () (Node',num2str(jj),'::RImmune2Age',num2str(k), ...
             ')  (* Node',num2str(jj),'nu (* Node',num2str(jj),'N (* ' ...
            '(- 1 (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType1 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),'))' ... 
            ' (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType2 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),')' ... 
            ' (- 1 (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType3 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),'))' ... 
             ' Node',num2str(jj),'AccessGroupPercent',num2str(j),'))))']};
         
             Equations(end+1,:) = {['(reaction BirthNode',num2str(jj),'R3Age',num2str(1),' () (Node',num2str(jj),'::RImmune3Age',num2str(k), ...
             ')  (* Node',num2str(jj),'nu (* Node',num2str(jj),'N (* ' ...
            '(- 1 (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType1 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),'))' ... 
            ' (- 1 (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType2 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),'))' ... 
            ' (* Node',num2str(jj),'gRoutAccess',num2str(j),' VaccineEfficacyTOPVType3 ',num2str(pa.DropOffRateRoutinePerAgeGroup(k)),')' ... 
             ' Node',num2str(jj),'AccessGroupPercent',num2str(j),'))))']};
        
        
            

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

if pa.SIADuration > 0
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
end

    Equations(end+1,:) = {['']};
    Equations(end+1,:) = {[';%%%%%% Node ',num2str(jj),' SIAs Time Dependent Reactions %%%%%%']};
    Equations(end+1,:) = {['']};
 
    if (pa.numOfSIAsPerNode(jj) > 0) && (pa.SIADuration ~= 0)
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
        
    elseif (pa.numOfSIAsPerNode(jj) > 0) && (pa.SIADuration == 0)
                            Equations(end+1,:) = {['(param tempSNode',num2str(jj),' 0)']};
                    Equations(end+1,:) = {['(param tempRImmune1Node',num2str(jj),' 0)']};
                    Equations(end+1,:) = {['(param tempRImmune2Node',num2str(jj),' 0)']};
                    Equations(end+1,:) = {['(param tempRImmune3Node',num2str(jj),' 0)']};
                    Equations(end+1,:) = {['(param tempRImmune12Node',num2str(jj),' 0)']};
                    Equations(end+1,:) = {['(param tempRImmune23Node',num2str(jj),' 0)']};
                    Equations(end+1,:) = {['(param tempRImmune13Node',num2str(jj),' 0)']};
                    
        for jjjjj = 1:pa.numOfSIAsPerNode(jj)
            for jjj = 1:pa.AccessibilityGroupNumbers
                for jjjj = 1:pa.numberOfAgeGroups
                    
                    if strcmp(pa.TypeOfSIACampaign,'Periodic')
                        eval(['Time = pa.PeriodicCampaignTimingVec.Node',num2str(jj),'(jjjjj);']);                     
                    else
                        eval(['Time = pa.NonPeriodicCampaignTiming.Node',num2str(jj),'(jjjjj);']);
                    end
                       
                    %Cov = ComputeGCCCoverage(pa,jj,jjjjj);
                    %eval(['CC = pa.GlobalSIACoverageVaccEffVec.Node',num2str(jj),'(jjjjj) * Cov(jjj)/pa.SIADuration;']);
                    eval(['WhichVaccine = pa.GlobalSIACoverageVaccEffVec.Node',num2str(jj),'(jjjjj);']);
                     eval(['GlobalCoverage = pa.GlobalSIACoverageVec.Node',num2str(jj),'(jjjjj);']); 
                    
                    if WhichVaccine == 1
                    
                        
%                         (param temp 0)
%                         (state-event whack-! (> I 50) ((Kv 0) (temp (* S 0.5)) (S (- S temp)) (V (+ V temp))))
%                         
%                     (time-event sia 50.0 ((Kv 0.02)))
%                     (time-event end 80.0 ((Kv 0)))
%                     

prob1 = (1 - pa.VaccineEfficacyTOPVType1) * (1 - pa.VaccineEfficacyTOPVType2) * (1 - pa.VaccineEfficacyTOPVType3);
                    prob2 = pa.VaccineEfficacyTOPVType1 * (1 - pa.VaccineEfficacyTOPVType2) * (1 - pa.VaccineEfficacyTOPVType3);
                    prob3 = pa.VaccineEfficacyTOPVType2 * (1 - pa.VaccineEfficacyTOPVType1) * (1 - pa.VaccineEfficacyTOPVType3);
                    prob4 = pa.VaccineEfficacyTOPVType3 * (1 - pa.VaccineEfficacyTOPVType1) * (1 - pa.VaccineEfficacyTOPVType2);
                    prob5 = pa.VaccineEfficacyTOPVType1 * (pa.VaccineEfficacyTOPVType2) * (1 - pa.VaccineEfficacyTOPVType3);
                    prob6 = pa.VaccineEfficacyTOPVType1 * (pa.VaccineEfficacyTOPVType3) * (1 - pa.VaccineEfficacyTOPVType2);
                    prob7 = pa.VaccineEfficacyTOPVType2 * (pa.VaccineEfficacyTOPVType3) * (1 - pa.VaccineEfficacyTOPVType1);
                    prob8 = pa.VaccineEfficacyTOPVType2 * (pa.VaccineEfficacyTOPVType3) * (pa.VaccineEfficacyTOPVType1);
                    
                    Equations(end+1,:) = {['(time-event ImpulsiveSSIATOPVPert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempSNode',num2str(jj),' Node',num2str(jj),'::S',num2str(jjj),'Age',num2str(jjjj),')'...
                        ' (Node',num2str(jj),'::S',num2str(jjj),'Age',num2str(jjjj),' (- Node',num2str(jj),'::S',num2str(jjj),'Age',num2str(jjjj),' (* tempSNode',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob1),'))))'...                        
                        ' (Node',num2str(jj),'::RImmune1Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune1Age',num2str(k),' (* tempSNode',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob2),')))' ...
                        ' (Node',num2str(jj),'::RImmune2Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune2Age',num2str(k),' (* tempSNode',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob3),')))' ...
                        ' (Node',num2str(jj),'::RImmune3Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune3Age',num2str(k),' (* tempSNode',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob4),')))' ...
                        ' (Node',num2str(jj),'::RImmune12Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune12Age',num2str(k),' (* tempSNode',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob5),')))' ...
                        ' (Node',num2str(jj),'::RImmune23Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune23Age',num2str(k),' (* tempSNode',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob7),')))' ...
                        ' (Node',num2str(jj),'::RImmune13Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune13Age',num2str(k),' (* tempSNode',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob6),')))' ...
                        ' (Node',num2str(jj),'::RImmune123Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune123Age',num2str(k),' (* tempSNode',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob8),')))' ...
                        '))']};
                    
                    prob9 = (1 - pa.VaccineEfficacyTOPVType2) * (1 - pa.VaccineEfficacyTOPVType3);
                    prob10 = pa.VaccineEfficacyTOPVType2 * (1 - pa.VaccineEfficacyTOPVType3);
                    prob11 = pa.VaccineEfficacyTOPVType3 * (1 - pa.VaccineEfficacyTOPVType2);
                    prob12 = (pa.VaccineEfficacyTOPVType2) * (pa.VaccineEfficacyTOPVType3);
                                      
                    Equations(end+1,:) = {['(time-event ImpulsiveR1SIATOPVPert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempRImmune1Node',num2str(jj),' Node',num2str(jj),'::RImmune1Age',num2str(k),')'...
                        ' (Node',num2str(jj),'::RImmune1Age',num2str(k),' (- Node',num2str(jj),'::RImmune1Age',num2str(k),' (* tempRImmune1Node',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob9),'))))'...
                        ' (Node',num2str(jj),'::RImmune12Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune12Age',num2str(k),' (* tempRImmune1Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob10),')))' ...
                        ' (Node',num2str(jj),'::RImmune13Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune13Age',num2str(k),' (* tempRImmune1Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob11),')))' ...
                        ' (Node',num2str(jj),'::RImmune123Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune123Age',num2str(k),' (* tempRImmune1Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob12),')))' ...
                        '))']};
                    
                    prob13 = (1 - pa.VaccineEfficacyTOPVType1) * (1 - pa.VaccineEfficacyTOPVType3);
                    prob14 = pa.VaccineEfficacyTOPVType1 * (1 - pa.VaccineEfficacyTOPVType3);
                    prob15 = pa.VaccineEfficacyTOPVType3 * (1 - pa.VaccineEfficacyTOPVType1);
                    prob16 = (pa.VaccineEfficacyTOPVType1) * (pa.VaccineEfficacyTOPVType3);
                    
                     Equations(end+1,:) = {['(time-event ImpulsiveR2SIATOPVPert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempRImmune2Node',num2str(jj),' Node',num2str(jj),'::RImmune2Age',num2str(k),')'...
                        ' (Node',num2str(jj),'::RImmune2Age',num2str(k),' (- Node',num2str(jj),'::RImmune2Age',num2str(k),' (* tempRImmune2Node',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob13),'))))'...
                        ' (Node',num2str(jj),'::RImmune12Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune12Age',num2str(k),' (* tempRImmune2Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob14),')))' ...
                        ' (Node',num2str(jj),'::RImmune23Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune23Age',num2str(k),' (* tempRImmune2Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob15),')))' ...
                        ' (Node',num2str(jj),'::RImmune123Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune123Age',num2str(k),' (* tempRImmune2Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob16),')))' ...
                        '))']};
                    
                    prob17 = (1 - pa.VaccineEfficacyTOPVType1) * (1 - pa.VaccineEfficacyTOPVType2);
                    prob18 = pa.VaccineEfficacyTOPVType1 * (1 - pa.VaccineEfficacyTOPVType2);
                    prob19 = pa.VaccineEfficacyTOPVType2 * (1 - pa.VaccineEfficacyTOPVType1);
                    prob20 = (pa.VaccineEfficacyTOPVType1) * (pa.VaccineEfficacyTOPVType2);
                    
                     Equations(end+1,:) = {['(time-event ImpulsiveR3SIATOPVPert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempRImmune3Node',num2str(jj),' Node',num2str(jj),'::RImmune3Age',num2str(k),')'...
                        ' (Node',num2str(jj),'::RImmune3Age',num2str(k),' (- Node',num2str(jj),'::RImmune3Age',num2str(k),' (* tempRImmune3Node',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob17),'))))'...
                        ' (Node',num2str(jj),'::RImmune23Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune23Age',num2str(k),' (* tempRImmune3Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob19),')))' ...
                        ' (Node',num2str(jj),'::RImmune13Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune13Age',num2str(k),' (* tempRImmune3Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob18),')))' ...
                        ' (Node',num2str(jj),'::RImmune123Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune123Age',num2str(k),' (* tempRImmune3Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob20),')))' ...
                        '))']};
                    
                    prob21 = (1 - pa.VaccineEfficacyTOPVType3) ;
                    prob22 = pa.VaccineEfficacyTOPVType3 ;
                    
                     Equations(end+1,:) = {['(time-event ImpulsiveR12SIATOPVPert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempRImmune12Node',num2str(jj),' Node',num2str(jj),'::RImmune12Age',num2str(k),')'...
                        ' (Node',num2str(jj),'::RImmune12Age',num2str(k),' (- Node',num2str(jj),'::RImmune12Age',num2str(k),' (* tempRImmune12Node',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob21),'))))'...
                        ' (Node',num2str(jj),'::RImmune123Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune123Age',num2str(k),' (* tempRImmune12Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob22),')))' ...
                        '))']};
                    
                    prob23 = (1 - pa.VaccineEfficacyTOPVType2) ;
                    prob24 = pa.VaccineEfficacyTOPVType2 ;
                    
                     Equations(end+1,:) = {['(time-event ImpulsiveR13SIATOPVPert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempRImmune13Node',num2str(jj),' Node',num2str(jj),'::RImmune13Age',num2str(k),')'...
                        ' (Node',num2str(jj),'::RImmune13Age',num2str(k),' (- Node',num2str(jj),'::RImmune13Age',num2str(k),' (* tempRImmune13Node',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob23),'))))'...
                        ' (Node',num2str(jj),'::RImmune123Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune123Age',num2str(k),' (* tempRImmune13Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob24),')))' ...
                        '))']};
                    
                    prob25 = (1 - pa.VaccineEfficacyTOPVType1) ;
                    prob26 = pa.VaccineEfficacyTOPVType1 ;
                    
                     Equations(end+1,:) = {['(time-event ImpulsiveR23SIATOPVPert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempRImmune23Node',num2str(jj),' Node',num2str(jj),'::RImmune23Age',num2str(k),')'...
                        ' (Node',num2str(jj),'::RImmune23Age',num2str(k),' (- Node',num2str(jj),'::RImmune23Age',num2str(k),' (* tempRImmune23Node',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob25),'))))'...
                        ' (Node',num2str(jj),'::RImmune123Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune123Age',num2str(k),' (* tempRImmune23Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob26),')))' ...
                        '))']};
    
                    Equations(end+1,:) = {['']};

                    elseif WhichVaccine == 2
                      
                    prob1 = (1 - pa.VaccineEfficacyBOPVType1) * (1 - pa.VaccineEfficacyBOPVType3);
                    prob2 = pa.VaccineEfficacyBOPVType1 * (1 - pa.VaccineEfficacyBOPVType3);
                    prob3 = pa.VaccineEfficacyBOPVType3 * (1 - pa.VaccineEfficacyBOPVType1);                    
                    prob4 = pa.VaccineEfficacyBOPVType1 * (pa.VaccineEfficacyBOPVType3);
                    
                    Equations(end+1,:) = {['(time-event ImpulsiveSSIABOPVPert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempSNode',num2str(jj),' Node',num2str(jj),'::S',num2str(jjj),'Age',num2str(jjjj),')'...
                        ' (Node',num2str(jj),'::S',num2str(jjj),'Age',num2str(jjjj),' (- Node',num2str(jj),'::S',num2str(jjj),'Age',num2str(jjjj),' (* tempSNode',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob1),'))))'...                        
                        ' (Node',num2str(jj),'::RImmune1Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune1Age',num2str(k),' (* tempSNode',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob2),')))' ...
                        ' (Node',num2str(jj),'::RImmune3Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune3Age',num2str(k),' (* tempSNode',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob3),')))' ...
                        ' (Node',num2str(jj),'::RImmune13Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune13Age',num2str(k),' (* tempSNode',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob4),')))' ...
                        '))']};
                    
                    prob5 = (1 - pa.VaccineEfficacyBOPVType3);
                    prob6 = pa.VaccineEfficacyBOPVType3 ;
                                      
                    Equations(end+1,:) = {['(time-event ImpulsiveR1SIABOPVPert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempRImmune1Node',num2str(jj),' Node',num2str(jj),'::RImmune1Age',num2str(k),')'...
                        ' (Node',num2str(jj),'::RImmune1Age',num2str(k),' (- Node',num2str(jj),'::RImmune1Age',num2str(k),' (* tempRImmune1Node',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob5),'))))'...
                        ' (Node',num2str(jj),'::RImmune13Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune13Age',num2str(k),' (* tempRImmune1Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob6),')))' ...
                          '))']};
                      
                    prob7 = (1 - pa.VaccineEfficacyBOPVType1);
                    prob8 = pa.VaccineEfficacyBOPVType1 ;
                                      
                    Equations(end+1,:) = {['(time-event ImpulsiveR3SIABOPVPert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempRImmune3Node',num2str(jj),' Node',num2str(jj),'::RImmune3Age',num2str(k),')'...
                        ' (Node',num2str(jj),'::RImmune3Age',num2str(k),' (- Node',num2str(jj),'::RImmune3Age',num2str(k),' (* tempRImmune3Node',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob7),'))))'...
                        ' (Node',num2str(jj),'::RImmune13Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune13Age',num2str(k),' (* tempRImmune3Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob8),')))' ...
                          '))']};
                    
                                      
                    Equations(end+1,:) = {['(time-event ImpulsiveR23SIABOPVPert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempRImmune23Node',num2str(jj),' Node',num2str(jj),'::RImmune23Age',num2str(k),')'...
                        ' (Node',num2str(jj),'::RImmune23Age',num2str(k),' (- Node',num2str(jj),'::RImmune23Age',num2str(k),' (* tempRImmune23Node',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob7),'))))'...
                        ' (Node',num2str(jj),'::RImmune123Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune123Age',num2str(k),' (* tempRImmune23Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob8),')))' ...
                          '))']};
                      
                      
                     Equations(end+1,:) = {['(time-event ImpulsiveR12SIABOPVPert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempRImmune12Node',num2str(jj),' Node',num2str(jj),'::RImmune12Age',num2str(k),')'...
                        ' (Node',num2str(jj),'::RImmune12Age',num2str(k),' (- Node',num2str(jj),'::RImmune12Age',num2str(k),' (* tempRImmune12Node',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob5),'))))'...
                        ' (Node',num2str(jj),'::RImmune123Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune123Age',num2str(k),' (* tempRImmune12Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob6),')))' ...
                          '))']};
                      
                    Equations(end+1,:) = {['']};
                        
                    elseif WhichVaccine == 3
                        
                    prob1 = (1 - pa.VaccineEfficacyMOPVType1);
                    prob2 = pa.VaccineEfficacyMOPVType1;
                    
                    Equations(end+1,:) = {['(time-event ImpulsiveSSIAMOPV1Pert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempSNode',num2str(jj),' Node',num2str(jj),'::S',num2str(jjj),'Age',num2str(jjjj),')'...
                        ' (Node',num2str(jj),'::S',num2str(jjj),'Age',num2str(jjjj),' (- Node',num2str(jj),'::S',num2str(jjj),'Age',num2str(jjjj),' (* tempSNode',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob1),'))))'...                        
                        ' (Node',num2str(jj),'::RImmune1Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune1Age',num2str(k),' (* tempSNode',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob2),')))' ...
                        '))']};
                        
                    Equations(end+1,:) = {['(time-event ImpulsiveR1SIAMOPV1Pert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempRImmune2Node',num2str(jj),' Node',num2str(jj),'::RImmune2Age',num2str(k),')'...
                        ' (Node',num2str(jj),'::RImmune2Age',num2str(k),' (- Node',num2str(jj),'::RImmune2Age',num2str(k),' (* tempRImmune2Node',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob1),'))))'...
                        ' (Node',num2str(jj),'::RImmune12Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune12Age',num2str(k),' (* tempRImmune2Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob2),')))' ...
                          '))']};
                        
                    Equations(end+1,:) = {['(time-event ImpulsiveR3SIAMOPV1Pert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempRImmune3Node',num2str(jj),' Node',num2str(jj),'::RImmune3Age',num2str(k),')'...
                        ' (Node',num2str(jj),'::RImmune3Age',num2str(k),' (- Node',num2str(jj),'::RImmune3Age',num2str(k),' (* tempRImmune3Node',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob1),'))))'...
                        ' (Node',num2str(jj),'::RImmune13Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune13Age',num2str(k),' (* tempRImmune3Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob2),')))' ...
                          '))']};
                      
                    Equations(end+1,:) = {['(time-event ImpulsiveR23SIAMOPV1Pert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempRImmune23Node',num2str(jj),' Node',num2str(jj),'::RImmune23Age',num2str(k),')'...
                        ' (Node',num2str(jj),'::RImmune23Age',num2str(k),' (- Node',num2str(jj),'::RImmune23Age',num2str(k),' (* tempRImmune23Node',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob1),'))))'...
                        ' (Node',num2str(jj),'::RImmune123Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune123Age',num2str(k),' (* tempRImmune23Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob2),')))' ...
                          '))']};
                        
                        
                        
                    elseif WhichVaccine == 4
                        
                    prob1 = (1 - pa.VaccineEfficacyMOPVType3);
                    prob2 = pa.VaccineEfficacyMOPVType3;
                    
                    Equations(end+1,:) = {['(time-event ImpulsiveSSIAMOPV3Pert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempSNode',num2str(jj),' Node',num2str(jj),'::S',num2str(jjj),'Age',num2str(jjjj),')'...
                        ' (Node',num2str(jj),'::S',num2str(jjj),'Age',num2str(jjjj),' (- Node',num2str(jj),'::S',num2str(jjj),'Age',num2str(jjjj),' (* tempSNode',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob1),'))))'...                        
                        ' (Node',num2str(jj),'::RImmune3Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune3Age',num2str(k),' (* tempSNode',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob2),')))' ...
                        '))']};
                        
                    Equations(end+1,:) = {['(time-event ImpulsiveR2SIAMOPV3Pert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempRImmune2Node',num2str(jj),' Node',num2str(jj),'::RImmune2Age',num2str(k),')'...
                        ' (Node',num2str(jj),'::RImmune2Age',num2str(k),' (- Node',num2str(jj),'::RImmune2Age',num2str(k),' (* tempRImmune2Node',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob1),'))))'...
                        ' (Node',num2str(jj),'::RImmune23Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune23Age',num2str(k),' (* tempRImmune2Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob2),')))' ...
                          '))']};
                        
                    Equations(end+1,:) = {['(time-event ImpulsiveR1SIAMOPV3Pert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempRImmune1Node',num2str(jj),' Node',num2str(jj),'::RImmune1Age',num2str(k),')'...
                        ' (Node',num2str(jj),'::RImmune1Age',num2str(k),' (- Node',num2str(jj),'::RImmune1Age',num2str(k),' (* tempRImmune1Node',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob1),'))))'...
                        ' (Node',num2str(jj),'::RImmune13Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune13Age',num2str(k),' (* tempRImmune1Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob2),')))' ...
                          '))']};
                      
                    Equations(end+1,:) = {['(time-event ImpulsiveR12SIAMOPV3Pert',num2str(jjjjj),'Node',num2str(jj),'Access',num2str(jjj),'Age',num2str(jjjj),...
                        ' ',num2str(Time),...
                        ' ((tempRImmune12Node',num2str(jj),' Node',num2str(jj),'::RImmune12Age',num2str(k),')'...
                        ' (Node',num2str(jj),'::RImmune12Age',num2str(k),' (- Node',num2str(jj),'::RImmune12Age',num2str(k),' (* tempRImmune12Node',num2str(jj),' ',num2str(GlobalCoverage),' (- 1 ',num2str(prob1),'))))'...
                        ' (Node',num2str(jj),'::RImmune123Age',num2str(jjjj),' (+ Node',num2str(jj),'::RImmune123Age',num2str(k),' (* tempRImmune12Node',num2str(jj),' ',num2str(GlobalCoverage),' ',num2str(prob2),')))' ...
                          '))']};                     
                        
                    end
                                        
                end
            end
        end
        
    end


%% W Stuff
    
%for j = 1:pa.AccessibilityGroupNumbers
    
    for jjj = 1 : pa.HowManyWildTypes

        if jjj == 1
        
    for k = 1:pa.numberOfAgeGroups
        
        Equations(end+1,:) = {['']};
        Equations(end+1,:) = {['; %%%%% Node',num2str(jj),'WType',num2str(jjj),'Age',num2str(k),' dot %%%%%% ']};
        Equations(end+1,:) = {['']};
        for kkk = 1:pa.InfectedWildComp

            Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
            'ImmuneN) () (* muAge',num2str(k),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN))']};
        
            Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune2 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
            'Immune2) () (* muAge',num2str(k),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune2))']};
        
            Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune3 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
            'Immune3) () (* muAge',num2str(k),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune3))']};
        
            Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune23 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
            'Immune23) () (* muAge',num2str(k),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune23))']};

            if kkk ~= pa.InfectedWildComp
    
                Equations(end+1,:) = {['(reaction WTransitionNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'ImmuneN) (Node',num2str(jj),'::W',num2str(kkk+1),'Type',num2str(jjj),'Age',num2str(k), ...
                'ImmuneN) (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN))']};   
            
                Equations(end+1,:) = {['(reaction WTransitionNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune2 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune2) (Node',num2str(jj),'::W',num2str(kkk+1),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune2) (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune2))']};  
            
                Equations(end+1,:) = {['(reaction WTransitionNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune3 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune3) (Node',num2str(jj),'::W',num2str(kkk+1),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune3) (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune3))']};  
            
                Equations(end+1,:) = {['(reaction WTransitionNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune23 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune23) (Node',num2str(jj),'::W',num2str(kkk+1),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune23) (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune23))']};  
            else
    
                Equations(end+1,:) = {['(reaction WRecoveryNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'ImmuneN) (Node',num2str(jj),'::RImmune1Age',num2str(k), ...
                ') (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN))']};  
            
                Equations(end+1,:) = {['(reaction WRecoveryNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune2 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune2) (Node',num2str(jj),'::RImmune12Age',num2str(k), ...
                ') (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune2))']}; 
            
                Equations(end+1,:) = {['(reaction WRecoveryNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune3 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune3) (Node',num2str(jj),'::RImmune13Age',num2str(k), ...
                ') (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune3))']};  
            
                Equations(end+1,:) = {['(reaction WRecoveryNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune23 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune23) (Node',num2str(jj),'::RImmune123Age',num2str(k), ...
                ') (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune23))']};  
            
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
    
        elseif jjj == 2
            
                for k = 1:pa.numberOfAgeGroups
        
        Equations(end+1,:) = {['']};
        Equations(end+1,:) = {['; %%%%% Node',num2str(jj),'WType',num2str(jjj),'Age',num2str(k),' dot %%%%%% ']};
        Equations(end+1,:) = {['']};
        for kkk = 1:pa.InfectedWildComp

            Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
            'ImmuneN) () (* muAge',num2str(k),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN))']};
        
            Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune1 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
            'Immune1) () (* muAge',num2str(k),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune1))']};
        
            Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune3 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
            'Immune3) () (* muAge',num2str(k),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune3))']};
        
            Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune13 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
            'Immune13) () (* muAge',num2str(k),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune13))']};
 
             if kkk ~= pa.InfectedWildComp
     
                Equations(end+1,:) = {['(reaction WTransitionNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'ImmuneN) (Node',num2str(jj),'::W',num2str(kkk+1),'Type',num2str(jjj),'Age',num2str(k), ...
                'ImmuneN) (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN))']};   
            
                Equations(end+1,:) = {['(reaction WTransitionNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune1 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune1) (Node',num2str(jj),'::W',num2str(kkk+1),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune1) (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune1))']};  
            
                Equations(end+1,:) = {['(reaction WTransitionNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune3 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune3) (Node',num2str(jj),'::W',num2str(kkk+1),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune3) (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune3))']};  
            
                Equations(end+1,:) = {['(reaction WTransitionNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune13 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune13) (Node',num2str(jj),'::W',num2str(kkk+1),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune13) (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune13))']};  
             else
    
                Equations(end+1,:) = {['(reaction WRecoveryNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'ImmuneN) (Node',num2str(jj),'::RImmune2Age',num2str(k), ...
                ') (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN))']};  
            
                Equations(end+1,:) = {['(reaction WRecoveryNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune3 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune3) (Node',num2str(jj),'::RImmune23Age',num2str(k), ...
                ') (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune3))']}; 
            
                Equations(end+1,:) = {['(reaction WRecoveryNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune1 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune1) (Node',num2str(jj),'::RImmune12Age',num2str(k), ...
                ') (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune1))']};  
            
                Equations(end+1,:) = {['(reaction WRecoveryNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune13 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune13) (Node',num2str(jj),'::RImmune123Age',num2str(k), ...
                ') (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune13))']};  
%             
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
        else
            
             for k = 1:pa.numberOfAgeGroups
        
        Equations(end+1,:) = {['']};
        Equations(end+1,:) = {['; %%%%% Node',num2str(jj),'WType',num2str(jjj),'Age',num2str(k),' dot %%%%%% ']};
        Equations(end+1,:) = {['']};
        for kkk = 1:pa.InfectedWildComp

            Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
            'ImmuneN) () (* muAge',num2str(k),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN))']};
        
            Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune1 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
            'Immune1) () (* muAge',num2str(k),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune1))']};
        
            Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune2 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
            'Immune2) () (* muAge',num2str(k),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune2))']};
        
            Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune12 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
            'Immune12) () (* muAge',num2str(k),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune12))']};

            if kkk ~= pa.InfectedWildComp
    
                Equations(end+1,:) = {['(reaction WTransitionNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'ImmuneN) (Node',num2str(jj),'::W',num2str(kkk+1),'Type',num2str(jjj),'Age',num2str(k), ...
                'ImmuneN) (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN))']};   
            
                Equations(end+1,:) = {['(reaction WTransitionNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune1 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune1) (Node',num2str(jj),'::W',num2str(kkk+1),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune1) (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune1))']};  
            
                Equations(end+1,:) = {['(reaction WTransitionNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune2 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune2) (Node',num2str(jj),'::W',num2str(kkk+1),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune2) (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune2))']};  
            
                Equations(end+1,:) = {['(reaction WTransitionNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune12 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune12) (Node',num2str(jj),'::W',num2str(kkk+1),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune12) (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune12))']};  
            else
    
                Equations(end+1,:) = {['(reaction WRecoveryNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'ImmuneN) (Node',num2str(jj),'::RImmune3Age',num2str(k), ...
                ') (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN))']};  
            
                Equations(end+1,:) = {['(reaction WRecoveryNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune2 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune2) (Node',num2str(jj),'::RImmune23Age',num2str(k), ...
                ') (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune2))']}; 
            
                Equations(end+1,:) = {['(reaction WRecoveryNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune1 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune1) (Node',num2str(jj),'::RImmune13Age',num2str(k), ...
                ') (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune1))']};  
            
                Equations(end+1,:) = {['(reaction WRecoveryNode',num2str(jj),'W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune12 (Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k), ...
                'Immune12) (Node',num2str(jj),'::RImmune123Age',num2str(k), ...
                ') (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::W',num2str(kkk),'Type',num2str(jjj),'Age',num2str(k),'Immune12))']};  
            
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
            
        end
    end
%end


%     % V Stuff
% if pa.SIADuration > 0
% for j = 1:pa.AccessibilityGroupNumbers
%     
%     for k = 1:pa.numberOfAgeGroups
%         
% Equations(end+1,:) = {['']};
% Equations(end+1,:) = {[';%%%%% Node',num2str(jj),'VAge',num2str(k),' dot %%%%% ']};
%         Equations(end+1,:) = {['']};
% for kkk = 1:pa.InfectedVaccComp
% 
% Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'V',num2str(kkk),'Age',num2str(k),' (Node',num2str(jj),'::V',num2str(kkk),'Age',num2str(k), ...
%     ') () (* muAge',num2str(k),' Node',num2str(jj),'::V',num2str(kkk),'Age',num2str(k), ...
%     '))']};
% 
% if kkk ~= pa.InfectedVaccComp
%     
%     Equations(end+1,:) = {['(reaction VTransitionNode',num2str(jj),'V',num2str(kkk),'Age',num2str(k),' (Node',num2str(jj),'::V',num2str(kkk),'Age',num2str(k), ...
%     ') (Node',num2str(jj),'::V',num2str(kkk+1),'Age',num2str(k), ...
%     ') (* TransitionType',num2str(jjj),'W',num2str(kkk),' Node',num2str(jj),'::V',num2str(kkk),'Age',num2str(k), ...
%     '))']};
%        
% else
%     
%     Equations(end+1,:) = {['(reaction VRecoveryNode',num2str(jj),'V',num2str(kkk),'Age',num2str(k),' (Node',num2str(jj),'::V',num2str(kkk),'Age',num2str(k), ...
%     ') (Node',num2str(jj),'::RAge',num2str(k), ...
%     ') (* TransitionW',num2str(kkk),' Node',num2str(jj),'::V',num2str(kkk),'Age',num2str(k), ...
%     '))']};
% end
% if pa.numberOfAgeGroups > 1
% if k ~= pa.numberOfAgeGroups
% 
%     Equations(end+1,:) = {['(reaction AgingV',num2str(kkk),'Age',num2str(k),'Node',num2str(jj),' (Node',num2str(jj),'::V',num2str(kkk),...
%     'Age',num2str(k),') (Node',num2str(jj),'::V',num2str(kkk),...
%     'Age',num2str(k+1),')  (* alphaAge',num2str(k),' Node',num2str(jj),'::V',num2str(kkk),...
%     'Age',num2str(k),' ) )']};
%   
% else
% 
%     Equations(end+1,:) = {['(reaction AgingV',num2str(kkk),'Age',num2str(k),'Node',num2str(jj),' (Node',num2str(jj),'::V',num2str(kkk),...
%     'Age',num2str(k),') ()  (* alphaAge',num2str(k),' Node',num2str(jj),'::V',num2str(kkk),...
%     'Age',num2str(k),' ) )']};
%     
% end
% end
% Equations(end+1,:) = {['']};
% 
%         end
%     end
%     
% end
% 
% end
% R Stuff 
for k = 1:pa.numberOfAgeGroups
        
Equations(end+1,:) = {['']};
Equations(end+1,:) = {[';%%%%% Node',num2str(jj),'RAge',num2str(k),' dot %%%%%']};
        Equations(end+1,:) = {['']};
%%%%%%%%%%%%%%%%%
        Equations(end+1,:) = {['(reaction Infected2Node',num2str(jj),'RImmune1Age',num2str(k),' (Node',num2str(jj),'::RImmune1Age',num2str(k), ...
            ') (Node',num2str(jj),'::W',num2str(j),'Type',num2str(2),'Age',num2str(k),'Immune1) (* Node',num2str(jj),'::RImmune1Age',num2str(k), ...
            ' (/ Node',num2str(jj),'LambdaWType',num2str(2),'Age',num2str(k),' Node',num2str(jj),'N)))']};   
       
        Equations(end+1,:) = {['(reaction Infected3Node',num2str(jj),'RImmune1Age',num2str(k),' (Node',num2str(jj),'::RImmune1Age',num2str(k), ...
            ') (Node',num2str(jj),'::W',num2str(j),'Type',num2str(3),'Age',num2str(k),'Immune1) (* Node',num2str(jj),'::RImmune1Age',num2str(k), ...
            ' (/ Node',num2str(jj),'LambdaWType',num2str(3),'Age',num2str(k),' Node',num2str(jj),'N)))']}; 
        
        Equations(end+1,:) = {['(reaction Infected1Node',num2str(jj),'RImmune2Age',num2str(k),' (Node',num2str(jj),'::RImmune2Age',num2str(k), ...
            ') (Node',num2str(jj),'::W',num2str(j),'Type',num2str(1),'Age',num2str(k),'Immune2) (* Node',num2str(jj),'::RImmune2Age',num2str(k), ...
            ' (/ Node',num2str(jj),'LambdaWType',num2str(1),'Age',num2str(k),' Node',num2str(jj),'N)))']};   
       
        Equations(end+1,:) = {['(reaction Infected3Node',num2str(jj),'RImmune2Age',num2str(k),' (Node',num2str(jj),'::RImmune2Age',num2str(k), ...
            ') (Node',num2str(jj),'::W',num2str(j),'Type',num2str(3),'Age',num2str(k),'Immune2) (* Node',num2str(jj),'::RImmune2Age',num2str(k), ...
            ' (/ Node',num2str(jj),'LambdaWType',num2str(3),'Age',num2str(k),' Node',num2str(jj),'N)))']};   
        
        
        Equations(end+1,:) = {['(reaction Infected1Node',num2str(jj),'RImmune3Age',num2str(k),' (Node',num2str(jj),'::RImmune3Age',num2str(k), ...
            ') (Node',num2str(jj),'::W',num2str(j),'Type',num2str(1),'Age',num2str(k),'Immune3) (* Node',num2str(jj),'::RImmune3Age',num2str(k), ...
            ' (/ Node',num2str(jj),'LambdaWType',num2str(1),'Age',num2str(k),' Node',num2str(jj),'N)))']};   
       
        Equations(end+1,:) = {['(reaction Infected2Node',num2str(jj),'RImmune3Age',num2str(k),' (Node',num2str(jj),'::RImmune3Age',num2str(k), ...
            ') (Node',num2str(jj),'::W',num2str(j),'Type',num2str(2),'Age',num2str(k),'Immune3) (* Node',num2str(jj),'::RImmune3Age',num2str(k), ...
            ' (/ Node',num2str(jj),'LambdaWType',num2str(2),'Age',num2str(k),' Node',num2str(jj),'N)))']};
        
         Equations(end+1,:) = {['(reaction Infected3Node',num2str(jj),'RImmune12Age',num2str(k),' (Node',num2str(jj),'::RImmune12Age',num2str(k), ...
            ') (Node',num2str(jj),'::W',num2str(j),'Type',num2str(3),'Age',num2str(k),'Immune12) (* Node',num2str(jj),'::RImmune12Age',num2str(k), ...
            ' (/ Node',num2str(jj),'LambdaWType',num2str(3),'Age',num2str(k),' Node',num2str(jj),'N)))']};   
       
        Equations(end+1,:) = {['(reaction Infected2Node',num2str(jj),'RImmune13Age',num2str(k),' (Node',num2str(jj),'::RImmune13Age',num2str(k), ...
            ') (Node',num2str(jj),'::W',num2str(j),'Type',num2str(2),'Age',num2str(k),'Immune13) (* Node',num2str(jj),'::RImmune13Age',num2str(k), ...
            ' (/ Node',num2str(jj),'LambdaWType',num2str(2),'Age',num2str(k),' Node',num2str(jj),'N)))']};
        
        Equations(end+1,:) = {['(reaction Infected1Node',num2str(jj),'RImmune23Age',num2str(k),' (Node',num2str(jj),'::RImmune23Age',num2str(k), ...
            ') (Node',num2str(jj),'::W',num2str(j),'Type',num2str(1),'Age',num2str(k),'Immune23) (* Node',num2str(jj),'::RImmune23Age',num2str(k), ...
            ' (/ Node',num2str(jj),'LambdaWType',num2str(1),'Age',num2str(k),' Node',num2str(jj),'N)))']};
%%%%%%%%%%%%%%%        
Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'RImmune123Age',num2str(k),' (Node',num2str(jj),'::RImmune123Age',num2str(k), ...
    ') () (* muAge',num2str(k),' Node',num2str(jj),'::RImmune123Age',num2str(k), ...
    '))']};

Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'RImmune12Age',num2str(k),' (Node',num2str(jj),'::RImmune12Age',num2str(k), ...
    ') () (* muAge',num2str(k),' Node',num2str(jj),'::RImmune12Age',num2str(k), ...
    '))']};

Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'RImmune23Age',num2str(k),' (Node',num2str(jj),'::RImmune23Age',num2str(k), ...
    ') () (* muAge',num2str(k),' Node',num2str(jj),'::RImmune23Age',num2str(k), ...
    '))']};

Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'RImmune13Age',num2str(k),' (Node',num2str(jj),'::RImmune13Age',num2str(k), ...
    ') () (* muAge',num2str(k),' Node',num2str(jj),'::RImmune13Age',num2str(k), ...
    '))']};

Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'RImmune1Age',num2str(k),' (Node',num2str(jj),'::RImmune1Age',num2str(k), ...
    ') () (* muAge',num2str(k),' Node',num2str(jj),'::RImmune1Age',num2str(k), ...
    '))']};

Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'RImmune2Age',num2str(k),' (Node',num2str(jj),'::RImmune2Age',num2str(k), ...
    ') () (* muAge',num2str(k),' Node',num2str(jj),'::RImmune2Age',num2str(k), ...
    '))']};

Equations(end+1,:) = {['(reaction DeathNode',num2str(jj),'RImmune3Age',num2str(k),' (Node',num2str(jj),'::RImmune3Age',num2str(k), ...
    ') () (* muAge',num2str(k),' Node',num2str(jj),'::RImmune3Age',num2str(k), ...
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
m = [];
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
for jjj = 1: pa.HowManyWildTypes
for jjjj = 1:pa.InfectedWildComp
    for k = 1:pa.numberOfAgeGroups
        
%         Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'W',num2str(jjjj),'Age',num2str(k), ...
%             ' (Node',num2str(j),'::W',num2str(jjjj),'Age',num2str(k),') (Node',num2str(jj),'::W',num2str(jjjj),'Age',num2str(k),...
%             ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::W',num2str(jjjj),'Age',num2str(k),'))']};
%         m = m+1;
        
        if jjj == 1
            
            Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN' ...
            ' (Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN) (Node',num2str(jj),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN'...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN))']};
        m = m+1;
            Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune2' ...
            ' (Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune2) (Node',num2str(jj),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune2'...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune2))']};
        m = m+1;
            Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune3' ...
            ' (Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune3) (Node',num2str(jj),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune3'...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune3))']};
        m = m+1;
            Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune23' ...
            ' (Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune23) (Node',num2str(jj),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune23'...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune23))']};
        m = m+1;
            
        elseif jjj == 2
            
            Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN' ...
            ' (Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN) (Node',num2str(jj),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN'...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN))']};
        m = m+1;
            Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune1' ...
            ' (Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune1) (Node',num2str(jj),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune1'...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune1))']};
        m = m+1;
            Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune3' ...
            ' (Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune3) (Node',num2str(jj),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune3'...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune3))']};
        m = m+1;
            Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune13' ...
            ' (Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune13) (Node',num2str(jj),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune13'...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune13))']};
        m = m+1;
            
        else
            
        Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN' ...
            ' (Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN) (Node',num2str(jj),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN'...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'ImmuneN))']};
        m = m+1;
            Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune1' ...
            ' (Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune1) (Node',num2str(jj),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune1'...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune1))']};
        m = m+1;
            Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune2' ...
            ' (Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune2) (Node',num2str(jj),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune2'...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune2))']};
        m = m+1;
            Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune12' ...
            ' (Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune12) (Node',num2str(jj),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune12'...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::W',num2str(jjjj),'Type',num2str(jjj),'Age',num2str(k),'Immune12))']};
        m = m+1;
            
        end
        
    end
end
end
%end


% %for jjj = 1:pa.AccessibilityGroupNumbers
% for jjjj = 1:pa.InfectedVaccComp
%      for k = 1:pa.numberOfAgeGroups
% 
%              Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'V',num2str(jjjj),'Age',num2str(k), ...
%             ' (Node',num2str(j),'::V',num2str(jjjj),'Age',num2str(k),') (Node',num2str(jj),'::V',num2str(jjjj),'Age',num2str(k),...
%             ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::V',num2str(jjjj),'Age',num2str(k),'))']};
%         m = m+1;
%         
%      end
% end
% %end

for k = 1:pa.numberOfAgeGroups

            Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'RImmune1Age',num2str(k), ...
            ' (Node',num2str(j),'::RImmune1Age',num2str(k),') (Node',num2str(jj),'::RImmune1Age',num2str(k),...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::RImmune1Age',num2str(k),'))']};
            m = m+1;
            
            Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'RImmune2Age',num2str(k), ...
            ' (Node',num2str(j),'::RImmune2Age',num2str(k),') (Node',num2str(jj),'::RImmune2Age',num2str(k),...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::RImmune2Age',num2str(k),'))']};
            m = m+1;
            
             Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'RImmune3Age',num2str(k), ...
            ' (Node',num2str(j),'::RImmune3Age',num2str(k),') (Node',num2str(jj),'::RImmune3Age',num2str(k),...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::RImmune3Age',num2str(k),'))']};
            m = m+1;
            
                        Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'RImmune12Age',num2str(k), ...
            ' (Node',num2str(j),'::RImmune12Age',num2str(k),') (Node',num2str(jj),'::RImmune12Age',num2str(k),...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::RImmune12Age',num2str(k),'))']};
            m = m+1;
            
            Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'RImmune23Age',num2str(k), ...
            ' (Node',num2str(j),'::RImmune23Age',num2str(k),') (Node',num2str(jj),'::RImmune23Age',num2str(k),...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::RImmune23Age',num2str(k),'))']};
            m = m+1;
            
             Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'RImmune13Age',num2str(k), ...
            ' (Node',num2str(j),'::RImmune13Age',num2str(k),') (Node',num2str(jj),'::RImmune13Age',num2str(k),...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::RImmune13Age',num2str(k),'))']};
            m = m+1;
            
                         Equations(end+1,:) = {['(reaction Migration',num2str(j),num2str(jj),'RImmune123Age',num2str(k), ...
            ' (Node',num2str(j),'::RImmune123Age',num2str(k),') (Node',num2str(jj),'::RImmune123Age',num2str(k),...
            ') (* ',num2str(GeneralMigRate*param.StateMigRate(m)),' Node',num2str(j),'::RImmune123Age',num2str(k),'))']};
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