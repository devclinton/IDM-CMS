
TimeVec = zeros(length(stateList),1);
MeanVec = zeros(Funs.NSpecies,length(stateList));
StdVec = zeros(Funs.NSpecies,length(stateList));
PErad = zeros(length(stateList),1);
for ii = 1:length(stateList)
   TimeVec(ii) = stateList{ii}.t;
   
   runData = reshape(stateList{ii}.x, Funs.NSpecies, []);
   Wild = sum(runData(2:5,:),1);
   
   MeanVec(:,ii) = mean(runData(:,Wild>0),2);
   StdVec(:,ii) = std(runData(:,Wild>0),0,2);
   PErad(ii) = sum(Wild<1);
end

MeanVec = MeanVec.';
StdVec = StdVec.';

Results = struct('Time', TimeVec, 'Mean', MeanVec, 'Std', StdVec, 'PErad', PErad);
Combined = [TimeVec, MeanVec, StdVec, PErad];
save('SSA.dat', '-ascii', 'Combined')