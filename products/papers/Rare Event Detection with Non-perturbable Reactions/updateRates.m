% function to update rate

function [k, gamma] = updateRates(reaction_constant_vector, original_rcv, index_of_update, gamma, lambda)

k = reaction_constant_vector;
k_original = original_rcv;
k_proposed = k_original(index_of_update) ./ gamma(index_of_update);
k(index_of_update) = k(index_of_update) * (1-lambda) + k_proposed * lambda;
        