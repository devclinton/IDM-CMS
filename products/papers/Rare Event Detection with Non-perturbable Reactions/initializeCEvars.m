function [gamma prop_cutoff, num, denom] = initializeCEvars(M, beta_max, fixed_index, irr_index, bin_index)

gamma = ones(M, beta_max);
prop_cutoff = zeros(M, beta_max-1);
num = zeros(M, beta_max);
denom = zeros(M, beta_max);


prop_cutoff([irr_index fixed_index],1) = 1;
bi = linspace(0,1,beta_max+1);
for i=1:length(bin_index)
    prop_cutoff(bin_index(i),:) = bi(2:beta_max);
end
