% function to update rate

function intermediate_re = findIntermediateRareEvent(x_ext, ind, re_type, re_val)

% re_type = 1  smaller pop is better, sort ascend 
% re type = 2  bigger pop is better, sort descend

switch re_type
    case -1
        x_ext = sort(x_ext, 'ascend');
        intermediate_re = x_ext(ind);
        if intermediate_re < re_val
            fprintf('intermediate_re %d exceeded re_val %d\n', intermediate_re, re_val)
            intermediate_re = re_val;
        end
    case 1
        x_ext = sort(x_ext, 'descend');
        intermediate_re = x_ext(ind);
        if intermediate_re > re_val
            fprintf('intermediate_re %d exceeded re_val %d\n', intermediate_re, re_val)
            intermediate_re = re_val;
        end
    otherwise
        error('unknown rare event type. exiting...');
end