function printData(gamma, n, num, denom)

% print out gamma
M = length(gamma);

fprintf('\tGamma values are: \n');
% for i=1:M
fprintf(['\t', num2str(gamma),'\n']);
% end
fprintf(['\tn are: \t', num2str(n),'\n']);

if nargin == 2
    return
end

% print out num and denom
fprintf(['\tnum are: \t', num2str(num),'\n']);
fprintf(['\tdenom are: \t', num2str(denom),'\n']);

end

