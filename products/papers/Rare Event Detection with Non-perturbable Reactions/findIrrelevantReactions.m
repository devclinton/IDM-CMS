function [irr_rxn, gammas, flag] = findIrrelevantReactions(ic, Kce, rho, fixed_index, re_type, re_val, num_try)

flag = 0;
x0 = ic.x0;
t0 = ic.t0;
tf = ic.tf;
k  = ic.k_original;
re_index = ic.re_index;

M = length(ic.stoch_matrix(:,1));
gamma = ones(1, M);
gammas = zeros(num_try, M);
x_ext = zeros(1,Kce);

parfor i=1:Kce
    if (mod(i,Kce/10)==0)
        fprintf('i: %d\n',i);
    end
    
    [localMax, num_out, denom_out, counter_out] = solveOnce(ic, k, gamma, re_val, re_type);
    x_ext(i) = localMax;
end
int_rare = findIntermediateRareEvent(x_ext, floor(Kce*rho), re_type, re_val);
fprintf('Using int_rare = %d to determine irrelevant reactions with %d number of tries...\n', int_rare, num_try);

for j=1:num_try
    denom = 0;
    num = 0;
    fprintf('Iteration %d/%d \n',j, num_try);
    parfor i=1:Kce
        if (mod(i,Kce/10)==0)
            fprintf('\ti: %d \n',i);
        end
        
        [localMax, num_out, denom_out, counter_out] = solveOnce(ic, k, gamma, int_rare, re_type);
        num = num + num_out;
        denom = denom + denom_out;
    end
    
    gammas(j,:) = num ./ denom;
end

% test to see if there is high fluctuation
% is comparing the magnitude of mean to variance good way of determining
% irrelevant reactions?

% Step 1. See if (gamma > 1) and (gamma < 1) on different iterations
len1s = sum(gams>=1);
irr_rxn = find(len1s(find(0<len1s))<num_try);


if isempty(irr_rxn)
    fprintf('No irrelevant reactions are found. Proceeding with CEDBIR...\n');
else
    fprintf(['Reactions ',num2str(irr_rxn),' are detected as irrelevant. Biasing parameters for these reactions will be set to 1.\n']);
    if (ismember(fixed_index, irr_rxn))
        inds = find(ismember(fixed_index, irr_rxn) == 1);
        fprintf(['Imperturbable reaction indices ' num2str(fixed_index(inds)),' appear to be irrelevant to rare event observation.\n']);
        fprintf(['Run regular multilevel CE routine and set gamma_',num2str(fixed_index),' to 1.\n']);
        fprintf('Exiting CEDBIR...\n');
        flag = 1;
        return;
    end
end
