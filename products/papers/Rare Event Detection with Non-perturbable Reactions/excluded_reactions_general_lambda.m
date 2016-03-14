% cross entropy to learn a single gamma (reciprocal relationship) in
% reversible isomerization model
% true probability ~= 1.191E-05
clear
% clc

% lambda controls how aggressive we want to update the biasing parameter
% for the excluded reactions

% open input file containing model description
[FileName,PathName] = uigetfile({'*.mat',  'mat files (*.mat)'}, 'Select Matlab model input file','MultiSelect','off');
if(isnumeric(FileName)) % user pressed cancel
    display('User canceled')
    return
end
file_in = textscan(FileName,'%s%*[^mat]','Delimiter','.');
file_in = char(file_in{1});
if isempty(file_in)
    error('Invalid file name');
end
load(fullfile(PathName, FileName));

Kce = 1e5;
expo = log10(Kce);
% epsilon to test for each lambda
eps_ce_vec = [.2 .175 .15 0.125 0.1];
eps_final_vec = [0.1 .075 0.05];
len_eps_ce_vec = length(eps_ce_vec);
len_eps_final_vec = length(eps_final_vec);
% store biasing parameters for each lambda and given eps
gammas = zeros(length(x0),length(eps_ce_vec),length(eps_final_vec));
runTimes= zeros(len_eps_ce_vec, len_eps_final_vec);
numIters = zeros(len_eps_ce_vec, len_eps_final_vec);

% extract lambda from the .mat file
lambdastr = textscan(FileName,'%*s%*slambda%s','Delimiter','._');
lambdastr = char(lambdastr{1})
if length(lambdastr) == 2
    lambda = str2double(lambdastr) * 0.1;
else
    lambda = str2double(lambdastr);
end

% determine the direction of bias
if (x0(re_index) > re_val)
    re_type = -1;
else
    re_type = 1;
end

% run when the flag for correlation indices is true
flag_correlation = 0;
if flag_correlation
    int_rare = getIntRE(ic, re_type, Kce, rho)
    [gammas, corr_indices] = findCorrelatedIndices(ic, int_rare, re_type, 1e4, rho);
end

% set random number stream with specific seed
seed = seed + ceil(10000*rand);
s = RandStream('mt19937ar','Seed',seed);
RandStream.setGlobalStream(s);


% write messages and intermediate states to a diary file
% diary off
filename = [file_in, '_', datestr(now, 'mm-dd-hh-MM'),'_lambda',lambdastr,'_N',num2str(expo),'_seed',num2str(seed)];
diary([filename, '.txt'])
diary on

fprintf(['model ', file_in, ' with rare event = ', num2str(re_val), '\n']);
fprintf(['random number seed: ',num2str(seed), '\n']);
fprintf(['Kce: ', num2str(Kce), '   rho: ', num2str(rho), '\n']);



% formatted messages to print out
m1 = '\tcounter: %d, k1: %g  k2: %g \t 1/k2: %g \t gamma1: %g  gamma2: %g \n';
m2 = '\tConverged to the final rare event! Relative error: %g   eps_final: %g \n';
m3 = '\tConverged to the intermediate rare event! Relative error: %g   eps_ce: %g \n';

fprintf('Using lambda: %g\n', lambda);
for l = 1: len_eps_ce_vec
    for m = 1: len_eps_final_vec
        % start convergence routine
        tic;
        flag_conv = 1;
        max_iter = 10;
        iter = 0;
        iter2 = 0;
        iter3 = 0;
        totRun = 0;
        eps_ce = eps_ce_vec(l);
        eps_final = eps_final_vec(m);
        
        % original reaction rates
        k = k_original;
        gamma = ones(1,M);
        
        fprintf('\n\neps ce: %g \t eps final: %g \n', eps_ce, eps_final);
        
        % start multilevel ce
        while(iter < max_iter)
            % CE iteration 1
            iter = iter + 1;
            totRun = totRun + 1;
            counter = 0;
            denom = 0;
            num = 0;
            x_ext = zeros(1,Kce);
            
            fprintf('\niter %d \n', iter);
            fprintf('\tk 1: %g  k 2: %g \t\t gamma1: %g  gamma2: %g\n', k(1), k(2), gamma(1), gamma(2));
            
            parfor i=1:Kce
                if (mod(i,Kce/10)==0)
                    fprintf('i: %d\n',i);
                end
                
                [localMax, num_out, denom_out, counter_out] = solveOnce(ic, k, gamma, re_val, re_type);
                num = num + num_out;
                denom = denom + denom_out;
                counter = counter + counter_out;
                x_ext(i) = localMax;
            end
            int_rare = findIntermediateRareEvent(x_ext, floor(Kce*rho), re_type, re_val);
            
            
            if (int_rare == re_val)
                fprintf('First stage reached the rare event: %d \n', re_val);
                % since rare event is reached, we need to use eps_final for convergence test, not eps_ce
                [rel_error num_data] = convergenceTest(gamma, k, fixed_index, eps_final, counter, ceil(Kce*rho));
                
                if (rel_error)
                    fprintf(m1, counter, k(1), k(2), 1/k(fixed_index), gamma(1), gamma(2));
                    fprintf(m2, abs(gamma(fixed_index) - 1/k(fixed_index)) / gamma(fixed_index), eps_final);
                    
                    gammas(:, l, m) = gamma;
                    runTimes(l,m) = toc;
                    numIters(l,m) = totRun;
                    totRun
                    flag_conv = 0;
                    break;
                else
                    % since int_rare is picked with ceil(Kce*rho), it always has enough data when reaching this loop
                    fprintf('\n\tError (%g) is greater than eps_final(%g).\n',abs(gamma(fixed_index) - 1/k(fixed_index)) / gamma(fixed_index), eps_final);
                    fprintf(m1, counter, k(1), k(2), 1/k(fixed_index), gamma(1), gamma(2));
                    fprintf('\tUpdate gamma (not k) then proceed to the final iteration.\n');
                    gamma = num ./ denom;
                    fprintf(m1, counter, k(1), k(2), 1/k(fixed_index), gamma(1), gamma(2));
                    break;
                end
            else
                fprintf('\tIntermediate rare event: %d \n', int_rare);
            end
            
            % CE iteration 2
            counter = 0;
            denom = 0;
            num = 0;
            iter2 = 0;
            max_iter2 = 10;
            
            while (iter2 < max_iter2)
                iter2 = iter2 + 1;
                totRun = totRun + 1;
                fprintf('\titer %d - %d\n', iter, iter2);
                
                parfor i=1:Kce
                    if (mod(i,Kce/10)==0)
                        fprintf('\ti: %d \n',i);
                    end
                    
                    [localMax, num_out, denom_out, counter_out] = solveOnce(ic, k, gamma, int_rare, re_type);
                    num = num + num_out;
                    denom = denom + denom_out;
                    counter = counter + counter_out;
                end
                
                old_gamma = gamma;
                gamma = num ./ denom;
                [rel_error num_data] = convergenceTest(gamma, k, fixed_index, eps_ce, counter, ceil(Kce*rho));
                
                if (rel_error && num_data)
                    fprintf(m1, counter, k(1), k(2), 1/k(fixed_index), gamma(1), gamma(2));
                    fprintf(m3, abs(gamma(fixed_index) - 1/k(fixed_index)) / gamma(fixed_index), eps_ce);
                    break;
                end
                
                if (rel_error && ~num_data)
                    fprintf('\n\tWithin tolerance but not enough data.\n');
                else if (~rel_error)
                        fprintf('\n\tError (%g) is greater than eps_ce(%g).\n',abs(gamma(2) - 1/k(2)) / gamma(2), eps_ce);
                    end
                end
                fprintf('\tUpdate k then continue CE iteration 2.\n');
                [k, gamma] = updateRates(k, k_original, fixed_index, gamma, lambda);
                fprintf(m1, counter, k(1), k(2), 1/k(2), gamma(1), gamma(2));
                
                % roll back if error is greater
                if (iter2 == max_iter2)
                    rho = rho * .9;
                    if rho*Kce < 200
                        Kce = ceil(200 / rho);
                    end
                    max_iter2 = max_iter2 + 5;
                    fprintf('\n\nConvergence is too slow. Adjusting CE parameters...\n');
                    fprintf('new max_iter2: %d \t \t new rho: %g \t new Kce: %g\n', max_iter2, rho, Kce);
                    
                    if (max_iter2 > 15)
                        fprintf('\n\n*******  Warning: iter2 ran 15 times without converging to the intermediate rare event %d  *******\n', int_rare);
                        fprintf('*******  To prevent the routine from getting into an infinite loop, we are stopping the simulation... *******\n\n')
                        
                        eps_ce
                        eps_final
                        break
                    end
                end
            end
            
            if (iter == max_iter)
                rho = rho * .9;
                if rho*Kce < 200
                    Kce = ceil(200 / rho);
                end
                max_iter = max_iter + 5;
                fprintf('\n\nConvergence is too slow. Adjusting CE parameters...\n');
                fprintf('new max_iter: %d \t \t new rho: %g \t new Kce: %g\n', max_iter, rho, Kce);
                
                if (max_iter > 15)
                    fprintf('\n\n*******  Warning: iter ran 15 times without converging to the rare event %d  *******\n', re_val);
                    fprintf('*******  To prevent the routine from getting into an infinite loop, we are stopping the simulation... *******\n\n')
                    
                    eps_ce
                    eps_final
                    break
                end
            end
        end
        
        if flag_conv   % final convergence routine, reached only when int_rare == re_val
            iter3 = 0;
            max_iter3 = 5;
            no_try = 0;
            starting_gamma = gamma;
            starting_k = k;
            fprintf('\n\nstarting the final convergence sequence with %g...\n', int_rare);
            while(iter3 < max_iter3)
                % CE iteration 1
                iter3 = iter3 + 1;
                totRun = totRun + 1;
                fprintf('final iter %d \n',iter3);
                
                counter = 0;
                denom = 0;
                num = 0;
                parfor i=1:Kce
                    if (mod(i,Kce/10)==0)
                        fprintf('i: %d\n',i);
                    end
                    
                    [localMax, num_out, denom_out, counter_out] = solveOnce(ic, k, gamma, re_val, re_type);
                    num = num + num_out;
                    denom = denom + denom_out;
                    counter = counter + counter_out;
                end
                
                old_gamma = gamma;
                gamma = num ./ denom;
                [rel_error num_data] = convergenceTest(gamma, k, fixed_index, eps_final, counter, ceil(Kce*rho));
                
                if (rel_error && num_data)
                    fprintf(m1, counter, k(1), k(2), 1/k(fixed_index), gamma(1), gamma(2));
                    fprintf(m2, abs(gamma(fixed_index) - 1/k(fixed_index)) / gamma(fixed_index), eps_final);
                    
                    gammas(:, l, m) = gamma;
                    runTimes(l,m) = toc;
                    numIters(l,m) = totRun;
                    totRun
                    break;
                end
                
                if (rel_error && ~num_data)
                    fprintf('\tWithin tolerance but not enough data.\n');
                else if (~rel_error)
                        fprintf('\n\tError (%g) is greater than eps_final(%g).\n',abs(gamma(fixed_index) - 1/k(fixed_index)) / gamma(fixed_index), eps_final);
                    end
                end
                fprintf('\tUpdate k then continue the final CE iteration.\n');
                [k, gamma] = updateRates(k, k_original, fixed_index, gamma, lambda);
                fprintf(m1, counter, k(1), k(2), 1/k(fixed_index), gamma(1), gamma(2));
                
                if (no_try == 3)
                    fprintf('\n\n*******  Warning: iter3 restarted 3 times without converging to the rare event %d  *******\n', re_val);
                    fprintf('*******  To prevent the routine from getting into an infinite loop, we are stopping the simulation... *******\n\n')
                    
                    eps_ce
                    eps_final
                    break
                end
                
                if (iter3 == max_iter3)
                    fprintf('\n\n*******  Warning: iter3 ran 5 times without converging to the rare event %d  *******\n', re_val);
                    fprintf('*******  Adjust CE parameter values and try again... *******\n\n');
                    
                    eps_ce
                    eps_final
                    
                    iter3 = 0;
                    no_try = no_try + 1;
                    rho = rho * .8;
                    if rho*Kce < 200
                        Kce = ceil(200 / rho);
                    end
                    gamma = starting_gamma;
                    k = starting_k;
                    fprintf('\tnew rho: %g \t new Kce: %g \t k(1): %g \t k(2): %g \t gamma(1): %g \t gamma(2):%g\n', rho, Kce, k(1), k(2), gamma(1), gamma(2));
                end
            end
        end
    end
end
matfile = [filename, '_gammaMat'];%[file_in, '_lambda', lambdastr,'_N',num2str(expo)];
save(matfile, 'gammas', 'Kce', 'rho', 'runTimes', 'numIters', 'eps_ce_vec', 'eps_final_vec');

