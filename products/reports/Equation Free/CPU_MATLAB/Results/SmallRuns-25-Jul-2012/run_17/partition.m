function parts = partition(N, Npartitions)

parts = {};

for ii = 1:Npartitions
   parts{ii} = floor((ii-1)*N/Npartitions)+1:floor(ii*N/Npartitions);
end
