% function to update rate

function [localMin, localMax] = getMinAndMax(ic)

x0 = ic.x0;
t0 = ic.t0;
tf = ic.tf;
stoch_matrix = ic.stoch_matrix;
re_index = ic.re_index;
k = ic.k_original;

M = length(stoch_matrix(1,:));
a = zeros(1,M);
x = x0;
t = t0;
w = 1;

n = zeros(1,M);
localMin = x(re_index);
localMax = x(re_index);


while(t<tf)
    a = k .* x;
    a0 = sum(a);
    
    % time to the next reaction
    tau = log(1/rand) / a0;
    t = t + tau;
    
    if(t <= tf)
        if(rand > a(1)/a0)
            j=2;
        else
            j=1;
        end
        
        x = x + stoch_matrix(j,:);
        
        localMin = min(localMin, x(re_index));
        localMax = max(localMax, x(re_index));
    end
end