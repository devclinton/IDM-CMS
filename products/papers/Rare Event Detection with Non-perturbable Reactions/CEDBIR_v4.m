function CEDBIR_v4(k_default, Kce_default, re_val, re_type, fixed_index, irr_index, bin_index, rho_default, lambda, ic, eps_ce_vec, eps_final_vec, PathName, FileName)
% CEDBIR

% lambda controls how aggressive we want to update the biasing parameter
% for the excluded reactions
len_eps_ce_vec = length(eps_ce_vec);
len_eps_final_vec = length(eps_final_vec);

% variables to save in .mat file
gammas = cell(length(eps_ce_vec),length(eps_final_vec));
pcs = cell(length(eps_ce_vec),length(eps_final_vec));
runTimes= zeros(len_eps_ce_vec, len_eps_final_vec);
numIters = zeros(len_eps_ce_vec, len_eps_final_vec);
convTable = zeros(len_eps_ce_vec, len_eps_final_vec);
KceTable = zeros(len_eps_ce_vec, len_eps_final_vec);

M = length(ic.stoch_matrix(:,1));
% beta_max = 10;
beta_max = 1;
kappa_min = 20;

for l = 1: len_eps_ce_vec
    for m = 1: len_eps_final_vec
        % start convergence routine
        tic;
        k = k_default;
        rho = rho_default;
        Kce = Kce_default;
        
        flag_conv = 1;
        max_iter = 20;
        iter = 0;
        totRun = 0;
        KceTot = 0;
        eps_ce = eps_ce_vec(l);
        eps_final = eps_final_vec(m);
        
        fprintf('\n\neps ce: %g \t eps final: %g \n', eps_ce, eps_final);
        
        % set gamma and propensity_cutoff (bin edges) cell structure
        [gamma prop_cutoff, num, denom, n]  = initializeCEvars_v4(M, beta_max, fixed_index, irr_index, bin_index);
        
        % start multilevel ce
        while(iter < max_iter)
            % CE iteration 1
            iter = iter + 1;
            totRun = totRun + 1;
            
            fprintf('\niter %d \n', iter);
            fprintf(['\tk: ', num2str(k), '\n']);
            printGammaPC(gamma, prop_cutoff);
            
            % restructure the CE variables
            counter = 0;
            x_ext = zeros(1,Kce);
            maxRP = zeros(M, Kce);
            minRP = zeros(M, Kce);
            num([irr_index fixed_index]) = {0};
            denom([irr_index fixed_index]) = {0};
            n([irr_index fixed_index]) = {0};
            for i=1:length(bin_index)
                blen = min(beta_max, length(prop_cutoff{bin_index(i)})+1);
                fillers = zeros(1, blen);
                num(bin_index(i)) = {fillers};
                denom(bin_index(i)) = {fillers};
                n(bin_index(i)) = {fillers};
            end
            
            parfor i=1:Kce
                if (mod(i,Kce/10)==0)
                    fprintf('i: %d\n',i);
                end
                [localMax, num_out, denom_out, counter_out, n_out, minRelProp, maxRelProp]  = solveOnce_v4(ic, k, gamma, prop_cutoff, bin_index, [fixed_index irr_index], re_val, re_type);
                
                if counter_out
                    counter = counter + counter_out;
                    num = cellAddHelper(num, num_out);
                    denom = cellAddHelper(denom, denom_out);
                    n = cellAddHelper(n, n_out);
                end
                x_ext(i) = localMax;
                maxRP(:,i) = maxRelProp';
                minRP(:,i) = minRelProp';
            end
            KceTot = KceTot + Kce;
            int_rare = findIntermediateRareEvent(x_ext, floor(Kce*rho), re_type, re_val);
            
            if (int_rare == re_val)
                fprintf('First stage reached the rare event: %d \n', re_val);
                [rel_error num_data err] = convergenceTest(gamma, k, k_default, fixed_index, eps_final, counter, ceil(Kce*rho));
                
                if (rel_error)
                    % store information and move on to the next param combination
                    runTimes(l,m) = toc;
                    gammas(l, m) = {gamma};
                    pcs(l,m) = {prop_cutoff};
                    KceTable(l,m) = KceTot;
                    numIters(l,m) = totRun;
                    convTable(l,m) = 1;
                    flag_conv = 0;
                    
                    fprintf(['\tCounter: ', num2str(counter), '\t k: ', num2str(k), '\n']);
                    printGammaPC(gamma, prop_cutoff);
                    fprintf('printing N...\n');
                    for i=1:M
                        fprintf(['i: ', num2str(i),'  ni: ', num2str(n{i}),'\n']);
                    end
                    fprintf(['\n\tConverged to the final rare event! Relative error: ', num2str(err),'\t eps_final: ', num2str(eps_final), '\n']);
                    fprintf(['\tTotal time: ',num2str(runTimes(l,m)),'\t Total number of iterations: ', num2str(totRun), '\tTotal Kce: ', num2str(KceTot), ' \n']);
                    old_gamma = gamma;
                    old_gamma
                    [gamma, prop_cutoff, n] = mergeData(num, denom, prop_cutoff, n, outvec, [fixed_index bin_index]);
                    fprintf('After final merging...\n');
                    printGammaPC(gamma, prop_cutoff);
                    fprintf('printing N...\n');
                    for i=1:M
                        fprintf(['i: ', num2str(i),'  ni: ', num2str(n{i}),'\n']);
                    end
                    fprintf('\n');
                    break;
                else
                    % since int_rare is picked with ceil(Kce*rho), it always has enough data when reaching this loop
                    fprintf(['\n\tError (', num2str(err) ,') is greater than eps_final (', num2str(eps_final),').\n']);
                    fprintf(['\tCounter: ', num2str(counter), '\t k: ', num2str(k), '\n']);
                    printGammaPC(gamma, prop_cutoff, old_pc);
                    fprintf('printing N...\n');
                    for i=1:M
                        fprintf(['i: ', num2str(i),'  ni: ', num2str(n{i}),'\n']);
                    end
                    fprintf('\n\tUpdate gamma then proceed to the final iteration.\n');
                    k = updateRates_v4(k, k_default, fixed_index, gamma, lambda);
                    fprintf(['\tCounter: ', num2str(counter), '\t k: ', num2str(k), '\n']);
                    
                    outvec =  merge(n, kappa_min, [fixed_index irr_index]);
                    [gamma, prop_cutoff, n] = mergeData(num, denom, prop_cutoff, n, outvec, [fixed_index bin_index]);
                    old_pc = prop_cutoff;
                    printGammaPC(gamma, prop_cutoff, old_pc);
                    fprintf('printing N...\n');
                    for i=1:M
                        fprintf(['i: ', num2str(i),'  ni: ', num2str(n{i}),'\n']);
                    end
                    fprintf('\n');
                    break;
                end
            else
                fprintf('\n\tIntermediate rare event: %d \n', int_rare);
            end
            
            % CE iteration 2
            iter2 = 0;
            max_iter2 = 20;
            mergeflag = 1;
            
            % Perform the following once before a set of CE2
            minRPv = min(minRP,[],2);
            maxRPv = max(maxRP,[],2);
            old_pc = prop_cutoff;
            for i=1:length(bin_index)
                if beta_max==1
                    prop_cutoff{bin_index(i)} = 1;
                else
                    brange = linspace(minRPv(bin_index(i)),maxRPv(bin_index(i)),beta_max+1);
                    prop_cutoff{bin_index(i)} = brange(2:beta_max);
                end
            end
            
            while (iter2 < max_iter2)
                iter2 = iter2 + 1;
                totRun = totRun + 1;
                fprintf('\titer %d - %d\n', iter, iter2);
                
                % restructure the CE variables for every CE2
                counter = 0;
                num([irr_index fixed_index]) = {0};
                denom([irr_index fixed_index]) = {0};
                n([irr_index fixed_index]) = {0};
                for i=1:length(bin_index)
                    blen = min(beta_max,length(prop_cutoff{bin_index(i)})+1);
                    fillers = zeros(1, blen);
                    num(bin_index(i)) = {fillers};
                    denom(bin_index(i)) = {fillers};
                    n(bin_index(i)) = {fillers};
                end
                
                parfor i=1:Kce
                    %                 for i=1:Kce
                    if (mod(i,Kce/10)==0)
                        fprintf('\ti: %d \n',i);
                    end
                    
                    [num_out, denom_out, counter_out, n_out] = solveOnce_stage2_v4(ic, k, gamma, prop_cutoff, old_pc, bin_index, [fixed_index irr_index], int_rare, mergeflag);
                    if counter_out
                        counter = counter + counter_out; % counter++
                        num = cellAddHelper(num, num_out);
                        denom = cellAddHelper(denom, denom_out);
                        n = cellAddHelper(n, n_out);
                    end
                end
                KceTot = KceTot + Kce;
                
                printGammaPC(gamma, prop_cutoff, old_pc);
                fprintf('printing N...\n');
                for i=1:M
                    fprintf(['i: ', num2str(i),'  ni: ', num2str(n{i}),'\n']);
                end
                outvec =  merge(n, kappa_min, [fixed_index irr_index]);
                [gamma, prop_cutoff, n] = mergeData(num, denom, prop_cutoff, n, outvec, [fixed_index bin_index]);
                mergeflag = 0;
                old_pc = prop_cutoff;
                [rel_error num_data err] = convergenceTest(gamma, k, k_default, fixed_index, eps_ce, counter, ceil(Kce*rho));
                
                fprintf('\tAfter merging\n');
                printGammaPC(gamma, prop_cutoff, old_pc);
                fprintf('printing N...\n');
                for i=1:M
                    fprintf(['i: ', num2str(i),'  ni: ', num2str(n{i}),'\n']);
                end
                
                if (rel_error && num_data)
                    fprintf(['\tCounter: ', num2str(counter), '\t k: ', num2str(k), '\n']);
                    printGammaPC(gamma, prop_cutoff);
                    fprintf(['\n\tConverged to the intermediate rare event! Relative error: ', num2str(err), '\t eps_ce: ', num2str(eps_ce),'\n']);
                    break;
                end
                
                fprintf(['\tBefore updating the rates:\tk: ', num2str(k), '\n']);
                
                if (rel_error && ~num_data)
                    fprintf(['\n\tWithin tolerance but not enough data (',num2str(counter),').\n']);
                else if (~rel_error)
                        fprintf(['\n\tError (', num2str(err) ,') is greater than eps_ce (', num2str(eps_ce),').\n']);
                    end
                end
                fprintf('\tUpdate k then continue CE iteration 2.\n');
                k  = updateRates_v4(k, k_default, fixed_index, gamma, lambda);
                fprintf(['\tCounter: ', num2str(counter), '\t k: ', num2str(k), '\n']);
                
                % roll back if error is greater
                if (iter2 == max_iter2)
                    rho = rho * .9;
                    if rho*Kce < 200
                        Kce = ceil(200 / rho);
                    end
                    max_iter2 = max_iter2 + 5;
                    fprintf('\n\nConvergence is too slow. Adjusting CE parameters...\n');
                    fprintf('new max_iter2: %d \t \t new rho: %g \t new Kce: %g\n', max_iter2, rho, Kce);
                    
                    if (max_iter2 > 30)
                        runTimes(l,m) = toc;
                        numIters(l,m) = totRun;
                        KceTable(l,m) = KceTot;
                        flag_conv = 0;
                        
                        gamma
                        k
                        
                        fprintf('\n\n*******  Warning: iter2 ran 30 times without converging to the intermediate rare event %d  *******\n', int_rare);
                        fprintf('*******  To prevent the routine from getting into an infinite loop, we are stopping the simulation... *******\n\n')
                        fprintf(['\tTotal time: ',num2str(runTimes(l,m)),'\t Total number of iterations: ', num2str(totRun), '\t Total Kce: ', num2str(KceTot), ' \n']);
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
                
                if (max_iter > 30)
                    runTimes(l,m) = toc;
                    numIters(l,m) = totRun;
                    KceTable(l,m) = KceTot;
                    flag_conv = 0;
                    
                    gamma
                    k
                    
                    fprintf('\n\n*******  Warning: iter ran 30 times without converging to the rare event %d  *******\n', re_val);
                    fprintf('*******  To prevent the routine from getting into an infinite loop, we are stopping the simulation... *******\n\n')
                    fprintf(['\tTotal time: ',num2str(runTimes(l,m)),'\t Total number of iterations: ', num2str(totRun), '\t Total Kce: ', num2str(KceTot), ' \n']);
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
            starting_pc = prop_cutoff;
            Kce = Kce_default;
            rho = rho_default;
            fprintf('\n\nstarting the final convergence sequence with %g...\n', int_rare);
            while(iter3 < max_iter3)
                % CE iteration 1
                iter3 = iter3 + 1;
                totRun = totRun + 1;
                fprintf('final iter %d \n',iter3);
                
                counter = 0;
                num([irr_index fixed_index]) = {0};
                denom([irr_index fixed_index]) = {0};
                n([irr_index fixed_index]) = {0};
                
                for i=1:length(bin_index)
                    blen = length(gamma{bin_index(i)});
                    fillers = zeros(1, blen);
                    num(bin_index(i)) = {fillers};
                    denom(bin_index(i)) = {fillers};
                    n(bin_index(i)) = {fillers};
                end
                
                printGammaPC(gamma, prop_cutoff, old_pc)
                
                parfor i=1:Kce
                    if (mod(i,Kce/10)==0)
                        fprintf('i: %d\n',i);
                    end
                    
                    [num_out, denom_out, counter_out, n_out] = solveOnce_stage2_v4(ic, k, gamma, prop_cutoff, old_pc, bin_index, [fixed_index irr_index], re_val, mergeflag);
                    if counter_out
                        counter = counter + counter_out; % counter++
                        num = cellAddHelper(num, num_out);
                        denom = cellAddHelper(denom, denom_out);
                        n = cellAddHelper(n, n_out);
                    end
                end
                KceTot = KceTot + Kce;
                
                outvec =  merge(n, kappa_min, [fixed_index irr_index]);
                old_gamma = gamma;
                [gamma, prop_cutoff, n] = mergeData(num, denom, prop_cutoff, n, outvec, [fixed_index bin_index]);
                old_pc = prop_cutoff;
                [rel_error num_data err] = convergenceTest(gamma, k, k_default, fixed_index, eps_final, counter, ceil(Kce*rho));
                
                if (rel_error && num_data)
                    runTimes(l,m) = toc;
                    gammas(l, m) = {gamma};
                    pcs(l, m) = {prop_cutoff};
                    numIters(l,m) = totRun;
                    KceTable(l,m) = KceTot;
                    convTable(l,m) = 1;
                    
                    fprintf(['\tCounter: ', num2str(counter), '\t k: ', num2str(k), '\n']);
                    printGammaPC(gamma, prop_cutoff);
                    old_gamma
                    fprintf(['\tConverged to the final rare event! Relative error: ', num2str(err),'\t eps_final: ', num2str(eps_final), '\n']);
                    fprintf(['\tTotal time: ',num2str(runTimes(l,m)),'\t Total number of iterations: ', num2str(totRun), '\t Total Kce: ', num2str(KceTot), ' \n']);
                    break;
                end
                
                fprintf(['\tBefore updating the rates:\tk: ', num2str(k), '\n']);
                printGammaPC(gamma);
                fprintf('printing N...\n');
                for i=1:M
                    fprintf(['i: ', num2str(i),'  ni: ', num2str(n{i}),'\n']);
                end
                
                if (rel_error && ~num_data)
                    fprintf(['\n\tWithin tolerance but not enough data (',num2str(counter),').\n']);
                else if (~rel_error)
                        fprintf(['\n\tError (', num2str(err) ,') is greater than eps_final (', num2str(eps_final),').\n']);
                    end
                end
                fprintf('\tUpdate k then continue the final CE iteration.\n');
                k = updateRates_v4(k, k_default, fixed_index, gamma, lambda);
                fprintf(['\tCounter: ', num2str(counter), '\t k: ', num2str(k), '\n']);
                
                if (no_try == 3)
                    runTimes(l,m) = toc;
                    numIters(l,m) = totRun;
                    KceTable(l,m) = KceTot;
                    
                    fprintf('\n\n*******  Warning: iter3 restarted 3 times without converging to the rare event %d  *******\n', re_val);
                    fprintf('*******  To prevent the routine from getting into an infinite loop, we are stopping the simulation... *******\n\n')
                    fprintf(['\tTotal time: ',num2str(runTimes(l,m)),'\t Total number of iterations: ', num2str(totRun), '\t Total Kce: ', num2str(KceTot), ' \n']);
                    break
                end
                
                if (iter3 == max_iter3)
                    rho = rho * .9;
                    if rho*Kce < 200
                        Kce = ceil(200 / rho);
                    end
                    
                    fprintf('\n\n*******  Warning: iter3 ran %d times without converging to the rare event %d  *******\n',max_iter3, re_val);
                    fprintf('*******  Adjust CE parameter values and try again... *******\n\n');
                    max_iter3 = max_iter3 + 5;
                    iter3 = 0;
                    no_try = no_try + 1;
                    rho = rho * .9;
                    if rho*Kce < 200
                        Kce = ceil(200 / rho);
                    end
                    gamma = starting_gamma;
                    k = starting_k;
                    prop_cutoff = starting_pc;
                    fprintf(['\tnew rho: ',num2str(rho),'\t new Kce: ',num2str(Kce),'\t k: ',num2str(k),'\n']);
                    
                    printGammaPC(gamma);
                    
                    fprintf('\n');
                end
            end
        end
    end
end
mfile = [FileName, '_data'];%[file_in, '_lambda', lambdastr,'_N',num2str(expo)];
save(fullfile(PathName, FileName, mfile), 'gammas', 'Kce', 'rho', 'runTimes', 'numIters', 'eps_ce_vec', 'eps_final_vec', 'convTable', 'KceTable','counter','old_gamma');
end

function out = cellAddHelper(A, B)

out=cell(numel(A),1);
for j=1:numel(A)
    out{j} = A{j} + B{j};
end
end

