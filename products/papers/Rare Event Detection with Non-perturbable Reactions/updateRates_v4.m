% function to update rate
function k = updateRates_v4(k, k_default, index_of_update, gamma, lambda)

k_proposed = k_default(index_of_update) ./ gamma{index_of_update};
k(index_of_update) = k(index_of_update) * (1-lambda) + k_proposed * lambda;

