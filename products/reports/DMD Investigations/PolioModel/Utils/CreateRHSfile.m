function output = CreateRHSfile(IC,pa)

Equations(1,:)={['function [xdot] = RhsAccess(t,x,param,pa,f,g,Eventtimes)']};
Equations(end+1,:) = {['']};
Equations(end+1,:) = {['n = pa.numberOfAgeGroups;']};
Equations(end+1,:) = {['']};

m=1;
numberOfNodes = length(pa.Mig(:,1));

for jj = 1:numberOfNodes
for j = 1:pa.AccessibilityGroupNumbers
Equations(end+1,:) = {['Node',num2str(jj),'_S',num2str(j),'(:,1) = x(',num2str((m-1)),'*n+1:',num2str(m),'*n);']};
m=m+1;
end

%for jjj = 1:pa.AccessibilityGroupNumbers
for j = 1:pa.InfectedWildComp
Equations(end+1,:) = {['Node',num2str(jj),'_IW',num2str(j),'(:,1) = x(',num2str((m-1)),'*n+1:',num2str(m),'*n);']};
m=m+1;
end
%end


%for jjj = 1:pa.AccessibilityGroupNumbers
for j = 1:pa.InfectedVaccComp
Equations(end+1,:) = {['Node',num2str(jj),'_IV',num2str(j),'(:,1) = x(',num2str((m-1)),'*n+1:',num2str(m),'*n);']};
m=m+1;
end
%end

Equations(end+1,:) = {['Node',num2str(jj),'_R(:,1) = x(',num2str((m-1)),'*n+1:',num2str(m),'*n);']};
Equations(end+1,:) = {['']};

m=m+1;
end
Equations(end+1,:) = {['']};


for j = 1:numberOfNodes   
    %for jj = 1:pa.AccessibilityGroupNumbers
        for jjj = 1:pa.numberOfAgeGroups
            tempstr = {['']};
            tempstr = strcat(tempstr,['Node',num2str(j),'_IV_Age_',num2str(jjj),'Sum = (']);

                %for jjjj = 1:pa.AccessibilityGroupNumbers
    
                    for jjjjj = 1:pa.numberOfAgeGroups
    
                        %tempstr = strcat(tempstr,[' + param.betaV(',num2str(jjj),',',num2str(jjjjj),',',num2str(jj),',',num2str(jjjj),')./param.N(',num2str(j),') * (']);
                        tempstr = strcat(tempstr,[' + param.betaV(',num2str(jjj),',',num2str(jjjjj),')./param.N(',num2str(j),') * (']);
                        
                        for jjjjjj = 1:pa.InfectedVaccComp
                             if pa.WhichCompAreInfectious(jjjjjj) == 1
                                tempstr = strcat(tempstr,['+ Node',num2str(j),'_IV',num2str(jjjjjj),'(',num2str(jjjjj),')']);
                             end
                        end
                        
                        tempstr = strcat(tempstr,')');
                    end
                %end
                
                tempstr = strcat(tempstr,');');
                Equations(end+1,:) =tempstr;
        end
    %end
end


    
Equations(end+1,:) = {''};

% for jj = 1:numberOfNodes
%         
%     if isfield(pa,'RuralUrban')
%         if pa.RuralUrban(jj) == 'r'
%             Equations(end+1,:) = {['Node',num2str(jj),'_LambdaV = param.betaVrural./param.N.*Node',num2str(jj),'_IVSum;']};
%         else 
%             Equations(end+1,:) = {['Node',num2str(jj),'_LambdaV = param.betaVurban./param.N.*Node',num2str(jj),'_IVSum;']};
%         end
% 
%     else
%         
%         Equations(end+1,:) = {['Node',num2str(jj),'_LambdaV = param.betaV./param.N.*Node',num2str(jj),'_IVSum;']};
%     end
% 
% m=m+1;
% end


for j = 1:numberOfNodes   
    %for jj = 1:pa.AccessibilityGroupNumbers
        for jjj = 1:pa.numberOfAgeGroups
            tempstr = {['']};
            tempstr = strcat(tempstr,['Node',num2str(j),'_IW_Age_',num2str(jjj),'Sum = (']);

                %for jjjj = 1:pa.AccessibilityGroupNumbers
    
                    for jjjjj = 1:pa.numberOfAgeGroups
    
                        tempstr = strcat(tempstr,[' + param.betaW(',num2str(jjj),',',num2str(jjjjj),')./param.N(',num2str(j),') * (']);
                        
                        for jjjjjj = 1:pa.InfectedVaccComp
                             if pa.WhichCompAreInfectious(jjjjjj) == 1
                                 tempstr = strcat(tempstr,['+ Node',num2str(j),'_IW',num2str(jjjjjj),'(',num2str(jjjjj),')']);
                             end
                        end
                        
                        tempstr = strcat(tempstr,')');
                    end
                %end
                
                tempstr = strcat(tempstr,');');
                Equations(end+1,:) =tempstr;
        end
    %end
end

Equations(end+1,:) = {''};
% 
% for jj = 1:numberOfNodes
% tempstr = {['']};    
%     
% tempstr = strcat(tempstr,['Node',num2str(jj),'_IWSum = sum(']);   
% for j = 1:pa.InfectedWildComp
% tempstr = strcat(tempstr,[' + Node',num2str(jj),'_IW',num2str(j)]);
% end
% tempstr = strcat(tempstr,');');
% Equations(end+1,:) =tempstr;
% 
% end
Equations(end+1,:) = {''};

for j = 1:numberOfNodes   
   %for jj = 1:pa.AccessibilityGroupNumbers
       
    tempstr = {['']};
    tempstr = strcat(tempstr,['Node',num2str(j),'_LambdaW = [']);
    
        for jjj = 1:pa.numberOfAgeGroups
    
            tempstr = strcat(tempstr,'Node',num2str(j),'_IW_Age_',num2str(jjj),'Sum');
            if jjj ~= pa.numberOfAgeGroups
                tempstr = strcat(tempstr,';');
            end
    
        end
        tempstr = strcat(tempstr,'];');
        Equations(end+1,:) = tempstr;
   %end

end


for j = 1:numberOfNodes
            
   %for jj = 1:pa.AccessibilityGroupNumbers
       
    tempstr = {['']};
    tempstr = strcat(tempstr,['Node',num2str(j),'_LambdaV = [']);
    
        for jjj = 1:pa.numberOfAgeGroups
    
            tempstr = strcat(tempstr,'Node',num2str(j),'_IV_Age_',num2str(jjj),'Sum');
            if jjj ~= pa.numberOfAgeGroups
                tempstr = strcat(tempstr,'; ');
            end
    
        end
        tempstr = strcat(tempstr,'];');
        Equations(end+1,:) = tempstr;
   %end

end

% Equations(end+1,:) = {''};
% Equations(end+1,:) = {['f_SIA = f;']};  
% Equations(end+1,:) = {''};


tempstr1(1,:) = {['']};
for jj = 1:numberOfNodes
Equations(end+1,:) = {'%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%'};
Equations(end+1,:) = {['%%% Node - ',num2str(jj),' %%%']};  
Equations(end+1,:) = {'%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%'};
Equations(end+1,:) = {''};
    
for j = 1:pa.AccessibilityGroupNumbers

Equations(end+1,:) = {['Node',num2str(jj),'_S',num2str(j),'dot = - Node',num2str(jj),'_S',num2str(j), ...
    '.* Node',num2str(jj),'_LambdaW - Node',num2str(jj),'_S',num2str(j), ...
    '.* Node',num2str(jj),'_LambdaV - pa.DyingVector.* Node',num2str(jj),'_S',num2str(j), ...
    ' - f.Node',num2str(jj),'(:,',num2str(j),')',...
    '+ param.nu(',num2str(jj),').*param.N(',num2str(jj),').*(1-pa.VaccineEfficacy' ...
    '*pa.DropOffRateRoutinePerAgeGroup(1)*g.Node',num2str(jj),'(',num2str(j),')).*param.accessGroupPercent(',num2str(jj),',',num2str(j), ... 
    ').* param.BirthAgeVec + param.AgingMatrixNode',num2str(jj),' * Node',num2str(jj),'_S',num2str(j),';']};

tempstr1(end+1,:) = {['Node',num2str(jj),'_S',num2str(j),'dot;']};

end

Equations(end+1,:) = {''};

%for jjj = 1:pa.AccessibilityGroupNumbers
for j = 1:pa.InfectedWildComp
    
    if j == 1
        tempstr = {['']};
        tempst = {['']};
        for jjj = 1:pa.AccessibilityGroupNumbers
            if jjj == 1
            tempstr = strcat(tempstr,['(Node',num2str(jj),'_S',num2str(jjj)]);
            else
            tempstr = strcat(tempstr,[' + Node',num2str(jj),'_S',num2str(jjj)]);
             %tempstr = strcat(tempstr,tempst);
            end    
        end
        tempstr = strcat(tempstr,')');
        Equations(end+1,:) = {['Node',num2str(jj),'_IW',num2str(j),'dot = ']};
        Equations(end,:) = strcat(Equations(end,:),tempstr);
        Equations(end,:) = strcat(Equations(end,:),['.* Node',num2str(jj),'_LambdaW - pa.DyingVector.* Node',num2str(jj),'_IW',num2str(j),' + param.AgingMatrix*Node',num2str(jj),'_IW',num2str(j),' '...
            ' + param.DTMatrix(',num2str(j),',',num2str(j),') * Node',num2str(jj),'_IW',num2str(j),';']);
    else
        Equations(end+1,:) = {['Node',num2str(jj),'_IW',num2str(j),'dot =  - pa.DyingVector.* Node',num2str(jj),'_IW',num2str(j),' + param.AgingMatrix*Node',num2str(jj),'_IW',num2str(j),' '...
            ' + param.DTMatrix(',num2str(j),',',num2str(j),') * Node',num2str(jj),'_IW',num2str(j),' + param.DTMatrix(',num2str(j),',',num2str(j-1),') * Node',num2str(jj),'_IW',num2str(j-1),';']}; 
    end
    
    tempstr1(end+1,:) = {['Node',num2str(jj),'_IW',num2str(j),'dot;']};
    
end
%end
Equations(end+1,:) = {''};

%for jjj = 1:pa.AccessibilityGroupNumbers
for j = 1:pa.InfectedVaccComp
        if j == 1
        for jjjj = 1 : pa.AccessibilityGroupNumbers
        
            if jjjj == 1
            tempst = {['(Node',num2str(jj),'_S',num2str(jjjj)]};
            tempst2 = {['f.Node',num2str(jj),'(:,',num2str(jjjj),')']};
            elseif jjjj == pa.AccessibilityGroupNumbers
                tempst = strcat(tempst,[' + Node',num2str(jj),'_S',num2str(jjjj),');']);
                tempst2 = strcat(tempst2,[' + f.Node',num2str(jj),'(:,',num2str(jjjj),')']);
            else
                tempst = strcat(tempst,[' + Node',num2str(jj),'_S',num2str(jjjj)]);
                tempst2 = strcat(tempst2,[' + f.Node',num2str(jj),'(:,',num2str(jjjj),')']);
                
            end
            
        end
        end
    
    if j == 1   
        Equations(end+1,:) = {['Node',num2str(jj),'_IV',num2str(j),'dot = ']};
        Equations(end,:) = strcat(Equations(end,:),tempstr);
        
        Equations(end,:) = strcat(Equations(end,:),[' .* Node',num2str(jj),'_LambdaV - pa.DyingVector.* Node',num2str(jj),'_IV',num2str(j),' + param.AgingMatrix*Node',num2str(jj),'_IV',num2str(j),' '...
            ' + param.DTMatrix(',num2str(j),',',num2str(j),') * Node',num2str(jj),'_IV',num2str(j),' '...
            '+ ']);
        Equations(end,:) = strcat(Equations(end,:),tempst2);
        Equations(end,:) = strcat(Equations(end,:),['+ param.nu(',num2str(jj),').*param.N(',num2str(jj),').*(pa.VaccineEfficacy*pa.DropOffRateRoutinePerAgeGroup(1)*g.Node',num2str(jj),'(',num2str(jjj),')).*param.accessGroupPercent(',num2str(jj),',',num2str(j), ... 
            ').* param.BirthAgeVec + param.RoutineAgingMatrixNode',num2str(jj),' * ']);
        Equations(end,:) = strcat(Equations(end,:),tempst);
    else
        Equations(end+1,:) = {['Node',num2str(jj),'_IV',num2str(j),'dot =  - pa.DyingVector.* Node',num2str(jj),'_IV',num2str(j),' + param.AgingMatrix*Node',num2str(jj),'_IV',num2str(j),' ' ...
            ' + param.DTMatrix(',num2str(j),',',num2str(j),') * Node',num2str(jj),'_IV',num2str(j),' + param.DTMatrix(',num2str(j),',',num2str(j-1),') * Node',num2str(jj),'_IV',num2str(j-1),';']}; 
    end
    
    tempstr1(end+1,:) = {['Node',num2str(jj),'_IV',num2str(j),'dot;']};
end
%end

tempstr = {['']};
%for jjjj = 1 :pa.AccessibilityGroupNumbers
tempstr = strcat(tempstr,['+ param.DTMatrix(',num2str(j+1),',',num2str(j),') * Node',num2str(jj),'_IV',num2str(pa.InfectedVaccComp),' ' ...
    ' + param.DTMatrix(',num2str(j+1),',',num2str(j),') * Node',num2str(jj),'_IW',num2str(pa.InfectedWildComp),]);
%end

Equations(end+1,:) = {''};
Equations(end+1,:) = strcat(['Node',num2str(jj),'_Rdot = param.AgingMatrix * Node',num2str(jj),'_R - Node',num2str(jj),'_R', ...
    ' .*pa.DyingVector '],tempstr);

Equations(end,:) = strcat(Equations(end,:),';');

tempstr1(end+1,:) = {['Node',num2str(jj),'_Rdot']};

Equations(end+1,:) = {''};
Equations(end+1,:) = {''};

end

if numberOfNodes ~= 1

Equations(end+1,:) = {['%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%']};
Equations(end+1,:) = {'   %%% Node Migration %%%'};  
Equations(end+1,:) = {'%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%'};
Equations(end+1,:) = {''};

m=1;
for j = 1:pa.AccessibilityGroupNumbers
Equations(end+1,:) = {['StateMigFactor_S',num2str(j),' = param.StateMigRate(',num2str((m-1)),'*n+1:',num2str(m),'*n);']};
m=m+1;
end


%for j = 1:pa.AccessibilityGroupNumbers
    for jj = 1:pa.InfectedWildComp
        Equations(end+1,:) = {['StateMigFactor_W',num2str(jj),' = param.StateMigRate(',num2str((m-1)),'*n+1:',num2str(m),'*n);']};
        m=m+1;
    end
%end

%for j = 1:pa.AccessibilityGroupNumbers
    for jj = 1:pa.InfectedVaccComp
        Equations(end+1,:) = {['StateMigFactor_V',num2str(jj),' = param.StateMigRate(',num2str((m-1)),'*n+1:',num2str(m),'*n);']};
        m=m+1;
    end
%end
   
        Equations(end+1,:) = {['StateMigFactor_R',num2str(jj),' = param.StateMigRate(',num2str((m-1)),'*n+1:',num2str(m),'*n);']};
        m=m+1;




for jj = 1:numberOfNodes
    Equations(end+1,:) = {''};
    Equations(end+1,:) = {['   %%% Node ',num2str(jj),' Migration %%%']};  
        Equations(end+1,:) = {''};
    
for j = 1:pa.AccessibilityGroupNumbers
    
    tempstr2(1,:) = {['']};
    for jjj = 1 : numberOfNodes
        
        if  (jjj~=numberOfNodes)
        tempstr2 = strcat(tempstr2,[' + Node',num2str(jjj),'_S',num2str(j),' * pa.Mig(',num2str(jj),',',num2str(jjj),').* StateMigFactor_S',num2str(j),'']);        
        elseif (jjj == numberOfNodes)
        tempstr2 = strcat(tempstr2,[' + Node',num2str(jjj),'_S',num2str(j),' * pa.Mig(',num2str(jj),',',num2str(jjj),').* StateMigFactor_S',num2str(j),';']);
        end
        
    end
      
    %%% Ne
        
Equations(end+1,:) = strcat(['Node',num2str(jj),'_S',num2str(j),'dot = Node',num2str(jj),'_S',num2str(j),'dot'],tempstr2);

    tempstr2(1,:) = {['']};
    for jjj = 1 : numberOfNodes
        
        if  (jjj~=numberOfNodes)
        tempstr2 = strcat(tempstr2,[' - Node',num2str(jj),'_S',num2str(j),' * pa.Mig(',num2str(jjj),',',num2str(jj),') .* StateMigFactor_S',num2str(j),'']);        
        elseif (jjj == numberOfNodes)
        tempstr2 = strcat(tempstr2,[' - Node',num2str(jj),'_S',num2str(j),' * pa.Mig(',num2str(jjj),',',num2str(jj),').* StateMigFactor_S',num2str(j),';']);
        end
        
    end

Equations(end+1,:) = strcat(['Node',num2str(jj),'_S',num2str(j),'dot = Node',num2str(jj),'_S',num2str(j),'dot'],tempstr2);

Equations(end+1,:) = {''};

end

for j = 1:pa.InfectedWildComp

    
    %for jjjj = 1:pa.AccessibilityGroupNumbers
    tempstr2(1,:) = {['']};
    for jjj = 1 : numberOfNodes
        
        if  (jjj~=numberOfNodes)
        tempstr2 = strcat(tempstr2,[' + Node',num2str(jjj),'_IW',num2str(j),' * pa.Mig(',num2str(jj),',',num2str(jjj),').* StateMigFactor_W',num2str(j),'']);        
        elseif (jjj == numberOfNodes)
        tempstr2 = strcat(tempstr2,[' + Node',num2str(jjj),'_IW',num2str(j),' * pa.Mig(',num2str(jj),',',num2str(jjj),').* StateMigFactor_W',num2str(j),';']);
        end
        
    end
    Equations(end+1,:) = strcat(['Node',num2str(jj),'_IW',num2str(j),'dot = Node',num2str(jj),'_IW',num2str(j),'dot'],tempstr2);
    %end
      
    %%% Ne
    
     %for jjjj = 1:pa.AccessibilityGroupNumbers
          tempstr2(1,:) = {['']};
    for jjj = 1 : numberOfNodes
        
        if  (jjj~=numberOfNodes)
        tempstr2 = strcat(tempstr2,[' - Node',num2str(jj),'_IW',num2str(j),' * pa.Mig(',num2str(jjj),',',num2str(jj),').* StateMigFactor_W',num2str(j),'']);        
        elseif (jjj == numberOfNodes)
        tempstr2 = strcat(tempstr2,[' - Node',num2str(jj),'_IW',num2str(j),' * pa.Mig(',num2str(jjj),',',num2str(jj),').* StateMigFactor_W',num2str(j),';']);
        end
        
    end    
    Equations(end+1,:) = strcat(['Node',num2str(jj),'_IW',num2str(j),'dot = Node',num2str(jj),'_IW',num2str(j),'dot'],tempstr2);
     %end


Equations(end+1,:) = {''};

end

for j = 1:pa.InfectedVaccComp

        
         %for jjjj = 1:pa.AccessibilityGroupNumbers
             tempstr2(1,:) = {['']};
        for jjj = 1 : numberOfNodes
        
        if  (jjj~=numberOfNodes)
        tempstr2 = strcat(tempstr2,[' + Node',num2str(jjj),'_IV',num2str(j),' * pa.Mig(',num2str(jj),',',num2str(jjj),').* StateMigFactor_V',num2str(j),'']);        
        elseif (jjj == numberOfNodes)
        tempstr2 = strcat(tempstr2,[' + Node',num2str(jjj),'_IV',num2str(j),' * pa.Mig(',num2str(jj),',',num2str(jjj),').* StateMigFactor_V',num2str(j),';']);
        end
        
        end
            Equations(end+1,:) = strcat(['Node',num2str(jj),'_IV',num2str(j),'dot = Node',num2str(jj),'_IV',num2str(j),'dot'],tempstr2);
         %end
      
    %%% Ne
        


   
    %for jjjj = 1:pa.AccessibilityGroupNumbers
         tempstr2(1,:) = {['']};
    for jjj = 1 : numberOfNodes
         
        if  (jjj~=numberOfNodes)
        tempstr2 = strcat(tempstr2,[' - Node',num2str(jj),'_IV',num2str(j),' * pa.Mig(',num2str(jjj),',',num2str(jj),').* StateMigFactor_V',num2str(j),'']);        
        elseif (jjj == numberOfNodes)
        tempstr2 = strcat(tempstr2,[' - Node',num2str(jj),'_IV',num2str(j),' * pa.Mig(',num2str(jjj),',',num2str(jj),').* StateMigFactor_V',num2str(j),';']);
        end
        
    end    
    Equations(end+1,:) = strcat(['Node',num2str(jj),'_IV',num2str(j),'dot = Node',num2str(jj),'_IV',num2str(j),'dot'],tempstr2);
    %end


Equations(end+1,:) = {''};
     
end

tempstr2(1,:) = {['']};
    for jjj = 1 : numberOfNodes
        
        if  (jjj~=numberOfNodes)
        tempstr2 = strcat(tempstr2,[' + Node',num2str(jjj),'_R * pa.Mig(',num2str(jj),',',num2str(jjj),')']);        
        elseif (jjj == numberOfNodes)
        tempstr2 = strcat(tempstr2,[' + Node',num2str(jjj),'_R * pa.Mig(',num2str(jj),',',num2str(jjj),');']);
        end
        
    end
      
    %%% Ne
        
Equations(end+1,:) = strcat(['Node',num2str(jj),'_Rdot = Node',num2str(jj),'_Rdot'],tempstr2);

    tempstr2(1,:) = {['']};
    for jjj = 1 : numberOfNodes
        
        if  (jjj~=numberOfNodes)
        tempstr2 = strcat(tempstr2,[' - Node',num2str(jj),'_R * pa.Mig(',num2str(jjj),',',num2str(jj),')']);        
        elseif (jjj == numberOfNodes)
        tempstr2 = strcat(tempstr2,[' - Node',num2str(jj),'_R * pa.Mig(',num2str(jjj),',',num2str(jj),');']);
        end
        
    end    

Equations(end+1,:) = strcat(['Node',num2str(jj),'_Rdot = Node',num2str(jj),'_Rdot;'],tempstr2);

Equations(end+1,:) = {''};
Equations(end+1,:) = {''};

end

end
Equations(end+1,:) = {''};
Equations(end+1,:) = {''};

Equations(end+1,:) = {'xdot = ['};
for j = 2 : length(tempstr1(:,1))

if j ~= length(tempstr1(:,1))
Equations(end+1,:) = strcat('    ',tempstr1(j,:));
else
Equations(end+1,:) = strcat('    ',tempstr1(j,:));
Equations(end,:) = strcat(Equations(end,:),'];');
end

end
%Equations(end+1,:) = {'];'};
Equations(end+1,:) = {''};
Equations(end+1,:) = {'end'};

% open the file with write permission
fid = fopen('RHSAccess.m', 'w');
fprintf(fid, '%s \n', Equations{:});
fclose(fid);

end