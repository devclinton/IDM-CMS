% test on solve once
clear
clc

load('reversible_isom_lambda1.mat');
 
beta_max = 15;
k = k_default;
bin_index = 1;

pc = cell(2,1);
pc{1}  = [0.27861     0.33014     0.38166     0.43319     0.48472     0.53625     0.58778      0.6393     0.69083     0.74236     0.79389     0.84542     0.89694     0.94847];
pc{2} = [1];

gamma = cell(2,1);
gamma{1} = [2.0454      2.0454      1.5771       1.327      1.1747      1.0877      1.0503      1.0903      1.1027      1.0487      1.0228     0.99679     0.99433        1.14      1.0086];
gamma{2} = [.84];

old_pc = cell(2,1);
opc1 = linspace(0,1,beta_max+1);
old_pc{1} = opc1(2:beta_max);
old_pc{2} = 1;
old_pc{:};

single_index = fixed_index;

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

n(bin_index) = {zeros(1,beta_max)};
lambda(bin_index) = {zeros(1,beta_max)};
num_out(bin_index) = {zeros(1,beta_max)};
denom_out(bin_index) = {zeros(1,beta_max)};


% while(t<tf)
for pp=1:10
    
    fprintf('\n\n\n');
    pp
    
    
    % check for a rare event
    if (x(re_index) == re_val)
        counter_out =  1;
        for p=1:numel(num_out)
            num_out{p} = num_out{p} + n{p}*w;
            denom_out{p} = denom_out{p} + lambda{p}*w;
        end
        break;
    end
    
    % compute propensity
    for i=1:M
        a(i) = prod(x(find(reac_matrix(i,:)==1)))*k(i);
    end
    a0 = sum(a);
    
    x
    a
    a0
    
    % determine biasing parameter
    % for fixed and irrelevant reactions, there is only one gamma   
    b(single_index) = a(single_index) .* horzcat(gamma{single_index});
    
    for i=1:length(bin_index)
        bi = bin_index(i);
        opci = old_pc{bi}
        old_pci_len = length(opci); 
        pci = pc{bi}
        pii = a(bi)/a0
        
        gamma{bi}
        
        % find gamma index using old pc
        ind = -1;
        for j=1:old_pci_len
            if pii<opci(j)
                ind = j
                break
            end
        end
        if ind == -1
            ind = old_pci_len + 1
        end
        inds(bi) = ind;
        b(bi) = a(bi)*gamma{bi}(ind);
        
        % find relevant bin index
        bind = -1;
        for j=1:(beta_max-1)
            if pii<pci(j)
                bind = j
                break
            end
        end
        if bind == -1
            bind = beta_max
        end
        binds(bi) = bind;
    end
    b0 = sum(b);
   
    
    b
    b0
    inds
    binds
    
    
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
    
    j
    n{:}
    lambda{:}
    
end

