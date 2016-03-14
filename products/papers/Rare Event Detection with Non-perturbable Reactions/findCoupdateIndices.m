function [indices] = findCoupdateIndices(ic)

Kce = 1e5;
expo = log10(Kce);

seed = 101 + seed*rand;
s = RandStream('mt19937ar','Seed',seed);
RandStream.setGlobalStream(s);
% run the presimulation to compute indices of coupdate
tempMins = zeros(1,Kce);
tempMaxs = zeros(1,Kce)*realmax;
for i=1:Kce
    
    if (mod(i,Kce/10)==0)
        fprintf('i: %d\n',i);
    end    
    [localMin, localMax] = getMinAndMax(ic);    
    tempMins(i) = localMin;
    tempMaxs(i) = localMax;
end
threshold = ceil(rho*Kce);
tempMin = sort(tempMins);
tempMin = tempMin(threshold)
tempMax = sort(tempMaxs, 'descend');
tempMax = tempMax(threshold)

if tempMin(ceil
