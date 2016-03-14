function [gamma ] = computeGamma_v4(num, denom, bin_index)
M = length(num);
gamma = cell(1,M);

for i=1:M    
    if ismember(i, bin_index)
        ni = num{i};
        di = denom{i};
        gamma{i} = ni./di;
    else
        gamma{i} = 1;
    end
end
end
