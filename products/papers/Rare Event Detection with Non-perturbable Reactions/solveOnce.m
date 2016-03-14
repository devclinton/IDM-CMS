% function to update rate

function [localMax, num_out, denom_out, counter_out] = solveOnce(ic, k, gamma, re_val, re_type)

x0 = ic.x0;
t0 = ic.t0;
tf = ic.tf;
stoch_matrix = ic.stoch_matrix;
re_index = ic.re_index;
reac_matrix = ic.reac_matrix;

M = length(stoch_matrix(:,1));
a = zeros(1,M);
x = x0;
t = t0;
w = 1;

n = zeros(1,M);
lambda = zeros(1,M);
localMax = x(re_index);

num_out = 0;
denom_out = 0;
counter_out = 0;

while(t<tf)

    % compute propensity
    for i=1:M
        a(i) = prod(x(find(reac_matrix(i,:)==1)))*k(i);
    end
    a0 = sum(a);
    b = gamma .* a;    
    b0 = sum(b);
    
    % check for a rare event
    if (x(re_index) == re_val)
        counter_out =  1;
        num_out = w * n;
        denom_out =  w * lambda;
        break;
    end
    % time to the next reaction
    tau = log(1/rand) / b0;
    t = t + tau;
    
    if(t <= tf)        
        j = find(cumsum(b)/b0 > rand, 1);
        n(j) = n(j)+1;                
        x = x + stoch_matrix(j,:);
        
        if re_type == 1
            localMax = max(localMax, x(re_index));
        else
            localMax = min(localMax, x(re_index));
        end
        w = w * exp((b0-a0)*tau) / gamma(j);
    else
        tau = tf - (t-tau);
        w = w * exp((b0-a0)*tau);
    end
    
    lambda = lambda + a*tau;
end