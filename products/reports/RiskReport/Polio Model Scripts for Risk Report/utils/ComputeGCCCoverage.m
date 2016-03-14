function Cov = ComputeGCCCoverage(pa,k,SIANum)

    covVec = pa.Cov(:,1);
    
    Cov = zeros(1,pa.AccessibilityGroupNumbers);

    for j = 1 : pa.AccessibilityGroupNumbers
       
        eval(['Cov(j) = interp1(covVec,pa.Cov(:,1+j),pa.GlobalSIACoverageVec.Node',num2str(k),'(SIANum));']);
        
    end
    
end