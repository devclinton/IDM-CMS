function f = ComputeSIACoverage(param,pa,ExIC,Index,SIANodeMatrix) %#ok<INUSL>

f = [];

NodeStateLength = pa.numberOfAgeGroups * pa.AccessibilityGroupNumbers;

if strcmp(pa.TypeOfCoverage,'Standard')
    
    NumberOfSIAs = sum(SIANodeMatrix(1:Index,:),1); %#ok<NASGU>
    
    for j = 1 : pa.numNodes
               
       eval(['f.Node',num2str(j),' = zeros(1:pa.numberOfAgeGroups,pa.AccessibilityGroupNumbers);']);
       
       for jj = 1 : pa.AccessibilityGroupNumbers
           
           if SIANodeMatrix(Index,j) ~= 0
           
           index1 = 1+(jj-1)*pa.numberOfAgeGroups + (j - 1) * NodeStateLength; %#ok<NASGU>
           index2 = (jj)*pa.numberOfAgeGroups +(j - 1) * NodeStateLength; %#ok<NASGU>
           
           eval(['f.Node',num2str(j),'(1:pa.numberOfAgeGroups,',num2str(jj), ...
               ') = ExIC(index1 : index2,1) * pa.GlobalSIACoverageVec.Node',num2str(j),'(NumberOfSIAs(j)) / pa.SIADuration* ' ...
                    ' pa.GlobalSIACoverageVaccEffVec.Node',num2str(j),'(',num2str(NumberOfSIAs),');']);
           end          
       end             
    end    
        
elseif strcmp(pa.TypeOfCoverage,'Guillaume')
    
    NumberOfSIAs = sum(SIANodeMatrix(1:Index,:),1);
    
    for j = 1 : pa.numNodes
             
           eval(['f.Node',num2str(j),' = zeros(pa.numberOfAgeGroups,pa.AccessibilityGroupNumbers);']);
        
           for jj = 1 : pa.AccessibilityGroupNumbers
                if SIANodeMatrix(Index,j) ~= 0
               
                index1 = 1+(jj-1)*pa.numberOfAgeGroups + (j - 1) * NodeStateLength;
                index2 = (jj)*pa.numberOfAgeGroups +(j - 1)*NodeStateLength;
               
                Cov = ComputeGCCCoverage(pa,j,NumberOfSIAs(j));        
                eval(['f.Node',num2str(j),'(1:pa.numberOfAgeGroups,jj) = ExIC(index1 : index2) * Cov(jj) / pa.SIADuration * ' ...
                    ' pa.GlobalSIACoverageVaccEffVec.Node',num2str(j),'(',num2str(NumberOfSIAs(j)),');']);
                
                end
           
           end
    end   
    
else
   disp('I do not recognize the type of Coverage') 
end


end

