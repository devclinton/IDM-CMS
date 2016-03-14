% function to update rate

function  [localMax, minRelProp, maxRelProp] = solveOnce_v4_2(ic, k, gamma, pc, alpha, bin_index, single_index, re_val, re_type)

x0 = ic.x0;
t0 = ic.t0;
tf = ic.tf;
stoch_matrix = ic.stoch_matrix;
re_index = ic.re_index;
reac_matrix = ic.reac_matrix;

M = length(stoch_matrix(:,1));
a = zeros(1,M);
b = zeros(1,M);
x = x0;
t = t0;
t2 = 0;

minRelProp = ones(1,M)*inf;
maxRelProp = zeros(1,M);
localMax = x(re_index);

while((t - sqrt(t2)*alpha) <tf)
    
    % check for a rare event
    if (x(re_index) == re_val)
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
        end
        b(bi) = a(bi)*gamma{bi}(ind);
    end
    b0 = sum(b);
    
    % time to the next reaction
    t = t + 1/b0;
    t2 = t2 + 1/b0*1/b0;
    
    if((t - sqrt(t2)*alpha) <= tf)
        j = find(cumsum(b)/b0 > rand, 1);
        x = x + stoch_matrix(j,:);
        if re_type == 1
            localMax = max(localMax, x(re_index));
        else
            localMax = min(localMax, x(re_index));
        end
    end
end
end
