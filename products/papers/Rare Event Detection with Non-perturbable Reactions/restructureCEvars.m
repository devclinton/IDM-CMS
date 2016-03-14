function [gamma prop_cutoff, num, denom] = restructureCEvars(M, beta_max, fixed_index, irr_index, bin_index, minRelProp, maxRelProp)


% gamma = ones(M, beta_max);
prop_cutoff = zeros(M, beta_max-1);
num = zeros(M, beta_max);
denom = zeros(M, beta_max);


prop_cutoff([irr_index fixed_index],1) = 1;
for i=1:length(bin_index)
    brange = linspace(minRelProp(bin_index(i)),maxRelProp(bin_index(i)),beta_max+1);
    prop_cutoff(bin_index(i),:) = brange(2:beta_max);
end
