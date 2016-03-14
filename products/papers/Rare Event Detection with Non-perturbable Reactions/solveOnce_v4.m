% function to update rate

function  [localMax, num_out, denom_out, counter_out, n, minRelProp, maxRelProp] = solveOnce_v4(ic, k, gamma, pc, bin_index, single_index, re_val, re_type)
counter_out = 0;

x0 = ic.x0;
t0 = ic.t0;
tf = ic.tf;
stoch_matrix = ic.stoch_matrix;
re_index = ic.re_index;
reac_matrix = ic.reac_matrix;

M = length(stoch_matrix(:,1));
a = zeros(1,M);
b = zeros(1,M);
inds = ones(1,M);
x = x0;
t = t0;
w = 1;

n =  cell(M,1);
lambda =  cell(M,1);
num_out = cell(M,1);
denom_out = cell(M,1);

minRelProp = ones(1,M)*inf;
maxRelProp = zeros(1,M);
localMax = x(re_index);

for i=1:M
    glen = length(cell2mat(gamma(i)));
    n(i) = {zeros(1,glen)};
    lambda(i) = {zeros(1,glen)};
    num_out(i) = {zeros(1,glen)};
    denom_out(i) = {zeros(1,glen)};
end

while(t<tf)
    
    % check for a rare event
    if (x(re_index) == re_val)
        counter_out =  1;
        [num_out denom_out] = ceAddHelper(num_out, n, denom_out, lambda, w);
        break;
    end
    
    % compute propensity
    for i=1:M
        a(i) = prod(x(find(reac_matrix(i,:)==1)))*k(i);
    end
    % if the system contains reactions that involve multiple of the same
    % species, e.g., 2X -> Y, comment the above for loop and use the below
    % code 
    % a = computePropensity(reac_matrix, x, k);
    a0 = sum(a);
    
    minRelProp = min(minRelProp, a/a0);
    maxRelProp = max(maxRelProp, a/a0);
    
    % determine biasing parameter
    % for fixed and irrelevant reactions, there is only one gamma    
    b(single_index) = a(single_index) .*  horzcat(gamma{single_index});
    for i=1:length(bin_index)
        bi = bin_index(i);
        pci = pc{bi};
        pii = a(bi)/a0;
        
        if length(gamma{bi})==1
            ind = 1;
        else
            ind = -1;
            
            for j=1:length(pci)
                if pii<pci(j)
                    ind = j;
                    break
                end
            end
            if ind == -1
                ind = length(pci)+1;
            end
            inds(bi) = ind;
        end
        b(bi) = a(bi)*gamma{bi}(ind);
    end
    b0 = sum(b);
    
    % time to the next reaction
    tau = log(1/rand) / b0;
    t = t + tau;
    
    if(t <= tf)
        j = find(cumsum(b)/b0 > rand, 1);
        x = x + stoch_matrix(j,:);
        n{j}(inds(j)) = n{j}(inds(j))+1;
        if re_type == 1
            localMax = max(localMax, x(re_index));
        else
            localMax = min(localMax, x(re_index));
        end
        
        w = w * exp((b0-a0)*tau) / gamma{j}(inds(j));
    else
        tau = tf - (t-tau);
        w = w * exp((b0-a0)*tau);
    end
    for i=1:M
        lambda{i}(inds(i)) = lambda{i}(inds(i))+a(i)*tau;
    end
end
end
% (num_out, n, denom_out, lambda, w);
function [nout dout] = ceAddHelper(A, B, C, D, w)
len = numel(A);
nout=cell(len,1);
dout=cell(len,1);
for p=1:len
    nout{p} = A{p} + B{p}*w;
    dout{p} = C{p} + D{p}*w;
end

end

