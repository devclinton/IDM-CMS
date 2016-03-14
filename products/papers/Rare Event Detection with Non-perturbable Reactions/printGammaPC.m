function printGammaPC(gamma, pc, opc)

% print out gamma
M = length(gamma);

fprintf('\tGamma values are: \n');
for i=1:M
    gi = gamma{i};
    fprintf(['\ti: ',num2str(i), '\t', num2str(gi),'\n']);
end

if nargin == 1
    return
end

% print out propensity cutoff
fprintf('\tPC values are: \n');
for i=1:M
    pci = pc{i};
    fprintf(['\ti: ',num2str(i), '\t', num2str(pci),'\n']);
end

if nargin == 3
    % print out old propensity cutoff
    fprintf('\tOPC values are: \n');
    for i=1:M
        opci = opc{i};
        fprintf(['\ti: ',num2str(i), '\t', num2str(opci),'\n']);
    end
end
    
end

