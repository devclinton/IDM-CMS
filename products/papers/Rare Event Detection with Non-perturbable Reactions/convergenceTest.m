function [rel_error num_data err] = convergenceTest(gamma, k_current, k_default, fixed_index, eps, counter, counter_threshold)

% output value = 1 -> test passed

err = max( abs(k_current(fixed_index).*gamma{fixed_index}-k_default(fixed_index))./k_default(fixed_index) );
if(err <= eps)
    rel_error = 1;
else
    rel_error = 0;
end

if (counter > counter_threshold)
    num_data = 1;
else
    num_data = 0;
end

end


