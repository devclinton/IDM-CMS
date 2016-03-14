% function to update rate

function [num_out, denom_out, counter_out, n] = solveOnce_stage2_v4_2(ic, k, gamma, pc, old_pc, bin_index, single_index, re_val)

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
binds = ones(1,M);
x = x0;
t = t0;
w = 1;

n =  cell(1,M);
lambda =  cell(1,M);
num_out = cell(1,M);
denom_out = cell(1,M);

n(single_index) = {0};
lambda(single_index) = {0};
num_out(single_index) = {0};
denom_out(single_index) = {0};

for i=1:length(bin_index)
    blen = length(gamma{bin_index(i)});
    n(bin_index) = {zeros(1,blen)};
    lambda(bin_index) = {zeros(1,blen)};
    num_out(bin_index) = {zeros(1,blen)};
    denom_out(bin_index) = {zeros(1,blen)};
end

while(t<tf)
    % check for a rare event
    if (x(re_index) == re_val)
        counter_out =  1;
        num_out = ceAddHelper(num_out, n, w);
        denom_out = ceAddHelper(denom_out, lambda, w);
        break;
    end
    
    % compute propensity
    for i=1:M
        a(i) = prod(x(find(reac_matrix(i,:)==1)))*k(i);
    end
    a0 = sum(a);
    
    % determine biasing parameter
    % for fixed and irrelevant reactions, there is only one gamma
    b(single_index) = a(single_index) .* horzcat(gamma{single_index});
    
    for i=1:length(bin_index)
        bi = bin_index(i);
        opci = old_pc{bi};
        old_pci_len = length(opci);
        pci = pc{bi};
        pci_len = length(pci);
        pii = a(bi)/a0;
        
        % find gamma index using old pc
        if length(gamma{bi})==1
            ind = 1;
        else
            ind = -1;
            for j=1:old_pci_len
                if pii<opci(j)
                    ind = j;
                    break
                end
            end
            if ind == -1
                ind = old_pci_len + 1;
            end
        end
        inds(bi) = ind;
        b(bi) = a(bi)*gamma{bi}(ind);
        
        % find relevant bin index
        bind = -1;
        for j=1:pci_len
            if pii<pci(j)
                bind = j;
                break
            end
        end
        if bind == -1
            bind = pci_len;
        end
        binds(bi) = bind;
    end
    b0 = sum(b);
    
    % time to the next reaction
    tau = log(1/rand) / b0;
    t = t + tau;
    
    if(t <= tf)
        j = find(cumsum(b)/b0 > rand, 1);
        x = x + stoch_matrix(j,:);
        n{j}(binds(j)) = n{j}(binds(j)) + 1;
        w = w * exp((b0-a0)*tau) / gamma{j}(inds(j));
    else
        tau = tf - (t-tau);
        w = w * exp((b0-a0)*tau);
    end
    
    for i=1:M
        lambda{i}(binds(i)) = lambda{i}(binds(i)) + a(i)*tau;
    end
end
end

function out = ceAddHelper(A, B, w)

out=cell(numel(A),1);
for p=1:numel(A)
    out{p} = A{p} + B{p}*w;
end

end
