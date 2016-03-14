function ExIC = ExtractedSusc(IC,pa)

Equations(1,:) = {['n = pa.numberOfAgeGroups;']};
Equations(end+1,:) = {['']};
n = pa.numberOfAgeGroups;
m=1;

numberOfNodes = pa.numNodes;
ExIC = [];

for jj = 1:numberOfNodes
for j = 1:pa.AccessibilityGroupNumbers
Equations(end+1,:) = {['Node',num2str(jj),'_S',num2str(j),'(:,1) = x(',num2str((m-1)),'*n+1:',num2str(m),'*n);']};

ExIC = [ExIC; IC((m-1)*n+1:m*n)];
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




end