function [prop_cutoff, num, denom] = restructureCEvars_v4(M, beta_max, fixed_index, irr_index, bin_index, minRelProp, maxRelProp)

prop_cutoff = cell(1,M);
num = cell(1,M);
denom = cell(1,M);

prop_cutoff([irr_index fixed_index]) = {1};
num([irr_index fixed_index]) = {0};
denom([irr_index fixed_index]) = {0};
    

fillers = zeros(1, beta_max);
for i=1:length(bin_index)
    num(bin_index(i)) = {fillers};
    denom(bin_index(i)) = {fillers};
    if beta_max == 1
        prop_cutoff(bin_index(i)) = {1};
    else
        pcb = {linspace(minRelProp(bin_index(i)),maxRelProp(bin_index(i)),beta_max+1)};
        prop_cutoff(bin_index(i)) = {pcb{:}(2:beta_max)};
    end
end

end