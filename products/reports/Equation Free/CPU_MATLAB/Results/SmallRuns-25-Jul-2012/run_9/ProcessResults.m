TimeVec = zeros(length(stateList),1);
MeanVec = zeros(Funs.NSpecies,length(stateList));
StdVec = zeros(Funs.NSpecies,length(stateList));
EradVec = zeros(length(stateList),1);

for ii = 1:length(stateList)
   TimeVec(ii) = stateList{ii}.t;
   
   runData = Funs.lift(stateList{ii},10000);
   MeanVec(:,ii) = mean(runData.data,2);
   StdVec(:,ii) = std(runData.data,0,2);
   EradVec(ii) = stateList{ii}.x(end);
end

MeanVec = MeanVec.';
StdVec = StdVec.';

Results = struct('Time', TimeVec, 'Mean', MeanVec, 'Std', StdVec);
Combined = [TimeVec, MeanVec, StdVec, EradVec];
save('CovarianceMeanStd.dat', '-ascii', 'Combined')