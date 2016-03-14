function dwSSA(k_default, Kce_default, re_val, re_type, rho_default, ic, target, abs_tol, PathName, FileName)

tic;
k = k_default;
rho = rho_default;
Kce = Kce_default;
M = length(ic.stoch_matrix(:,1));
max_iter = 10;
iter = 0;
gamma = ones(1,M);

initial_size = 3;
ires = zeros(1,initial_size);
gammas = zeros(M, initial_size);
initGammas = zeros(M, initial_size);
ns = zeros(M, initial_size);
nums = zeros(M, initial_size);
denoms = zeros(M, initial_size);
counters = zeros(1,initial_size);

% start multilevel ce
while(iter < max_iter)
    % CE iteration 1
    iter = iter + 1;
    fprintf('\niter %d \n', iter);
    
    % restructure the CE variables
    counter = 0;
    x_ext = zeros(1,Kce);
    num = zeros(1,M);
    denom = zeros(1,M);
    n = zeros(1,M);
    initGammas(:,iter) = gamma';
    
    parfor i=1:Kce
        if (mod(i,Kce/10)==0)
            fprintf('i: %d\n',i);
        end
        [localMax, num_out, denom_out, counter_out, n_out] = solveOnce_v4(ic, k, gamma, re_val, re_type);
        
        if counter_out
            counter = counter + counter_out;
            num = num + num_out;
            denom = denom + denom_out;
            n = n + n_out;
        end
        x_ext(i) = localMax;
    end
    int_rare = findIntermediateRareEvent(x_ext, floor(Kce*rho), re_type, re_val);
    
    if (int_rare == re_val && counter >= floor(Kce*rho))
        fprintf('First stage reached the rare event: %d\t re counter: %d \n', re_val, counter);
                
        printData(gamma, n, num, denom);
        gammas(:,iter) = gamma';
        ns(:,iter) = n';
        nums(:,iter) = num';
        denoms(:,iter) = denom';
        ires(iter) = re_val;
        counters(iter) = counter;
        toc;
        break;
    else
        ires(iter) = int_rare;
        fprintf('\n\tIntermediate rare event: %d \n', int_rare);
    end
    
    % CE iteration 2    
    fprintf('\titer %d - 2\n', iter);
    
    % restructure the CE variables for every CE2
    counter = 0;
    num = zeros(1,M);
    denom = zeros(1,M);
    n = zeros(1,M);
    
    parfor i=1:Kce
        if (mod(i,Kce/10)==0)
            fprintf('\ti: %d \n',i);
        end
        
        [num_out, denom_out, counter_out, n_out] = solveOnce_stage2_v4(ic, k, gamma, int_rare);
        if counter_out
            counter = counter + counter_out;
            num = num + num_out;
            denom = denom + denom_out;
            n = n + n_out;
        end
    end
    
    fprintf('\n\tCE2: Intermediate rare event: %d \t counter: %d\n', int_rare, counter);
    printData(gamma, n, num, denom);
    gamma = num./denom;
    gammas(:,iter) = gamma';
    ns(:,iter) = n';
    nums(:,iter) = num';
    denoms(:,iter) = denom';
    counters(iter) = counter;
end

len = find(ires==0,1);
if ~isempty(len)
    len = len-1; %last nonzero element
    initGammas = initGammas(:,1:len);
    gammas = gammas(:,1:len);
    ns = ns(:,1:len);
    nums = nums(:,1:len);
    denoms = denoms(:,1:len);
    ires = ires(1:len);
    counters = counters(1:len);
end
fprintf(['\t\tIREs are: ', num2str(ires),'\n']);
mfile = [FileName, '_re_val_',num2str(re_val),'.mat'];
save(fullfile(PathName,FileName, mfile), 'gammas','initGammas', 'ns', 'ires', 'nums', 'denoms', 'counters','Kce', 'rho','re_val');

% solve4target(ic, target, abs_tol, re_val, gammas, counter, Kce, k, rho);

end

