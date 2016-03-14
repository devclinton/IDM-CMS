function [gamma prop_cutoff, num, denom, n] = initializeCEvars_v4(M, beta_max, fixed_index, irr_index, bin_index)

gamma = cell(M,1);
prop_cutoff = cell(M,1);
num = cell(M,1);
denom = cell(M,1);
n = cell(M,1);

gamma([irr_index fixed_index]) = {1};
gamma(bin_index) = {ones(1,beta_max)};

prop_cutoff([irr_index fixed_index]) = {1};
if beta_max==1
    prop_cutoff(bin_index) = {1};
else
    pcb = {linspace(0,1,beta_max+1)};
    prop_cutoff(bin_index) = {pcb{:}(2:beta_max)};
end

num([irr_index fixed_index]) = {0};
num(bin_index) = {zeros(1,beta_max)};

denom([irr_index fixed_index]) = {0};
denom(bin_index) = {zeros(1,beta_max)};

n([irr_index fixed_index]) = {0};
n(bin_index) = {zeros(1,beta_max)};
