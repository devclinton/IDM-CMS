function g = ComputeRoutineCoverage(param,pa,IC)

g = [];

if strcmp(pa.TypeOfCoverage,'Standard')
    
    for j = 1 : pa.numNodes
               
       eval(['g.Node',num2str(j),' = zeros(1,pa.AccessibilityGroupNumbers);']);
       
       for jj = 1 : pa.AccessibilityGroupNumbers
           
           eval(['g.Node',num2str(j),'(1,',num2str(jj),') = pa.GlobalRoutineCoverage(1,j);']);
           
       end
            
            
    end    
        
elseif strcmp(pa.TypeOfCoverage,'Guillaume')
    
    for j = 1 : pa.numNodes
               
       eval(['g.Node',num2str(j),' = zeros(1,pa.AccessibilityGroupNumbers);']);
                         
           Cov = ComputeCoverageGCC2(pa,j);
           
           eval(['g.Node',num2str(j),'(1,:) = Cov;']);
                     
    end   
    
else
   disp('I do not recognize the type of Coverage') 
end


end

function Cov = ComputeCoverageGCC2(pa,k)

    covVec = pa.Cov(:,1);
    
    Cov = zeros(1,pa.AccessibilityGroupNumbers);
    
    for j = 1 : pa.AccessibilityGroupNumbers
        
        eval(['Xi = pa.GlobalRoutineCoverageVec(1,',num2str(k),');']);
        eval(['Cov(j) = interp1(covVec,pa.Cov(:,1+j),Xi);']);
        
        if Cov(j) > 1
            Cov(j) = 1;
        elseif Cov(j) <0
            Cov(j) = 0;
        end
        
    end
    
end