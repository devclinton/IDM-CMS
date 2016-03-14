% cross entropy to learn a single gamma (reciprocal relationship) in
% reversible isomerization model
% true probability ~= 1.191E-05
clear
clc

% convergeMethod = 1
% run until converge during multilevel CE
% convergeMethod = 2
% run until converge AFTER CE stage is over (rare event is reached)


% updateMethod = 1
% direct division update without multiplying other reaction constants
% updateMethod = 2
% direct division update with multiplying other reaction constants
% updateMethod = 3
% averaged update without multiplying other reaction constants
% updateMethod = 4
% averaged update with multiplying other reaction constants


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

% store biasing parameters
umlen = 4;
cmlen = 2;
gammas = zeros(M,umlen,cmlen);


Kce = 1e6;
expo = log10(Kce);
% % % % % % % Make sure to insert result into the correct gamma matrix slot
% and also save it before exiting the function. Might need multiple save
% commands before return commands

seed = 1;
for cm = 1:cmlen
    convergeMethod = cm;
    
    for um =1:umlen
        
        tic;
        % set random number stream with specific seed
        seed = seed + 100;
        s = RandStream('mt19937ar','Seed',seed);
        RandStream.setGlobalStream(s);
        
        updateMethod = um;
        
        
        gamma = ones(1,M);
        k = k_original;
        
        % write messages and intermediate states to a diary file
        diary off
        filename = [file_in, '_', datestr(now, 'mm-dd-hh-MM'),'_cm',num2str(convergeMethod),'_um',num2str(updateMethod),'_N',num2str(expo),'_seed',num2str(seed),'_eps',epstr,'.txt'];
        diary(filename)
        diary on
        
        
        fprintf(['model ', file_in, ' with rare event = ', num2str(re_val), '\n']);
        fprintf(['random number seed: ',num2str(seed), '\n']);
        fprintf(['update method: ', num2str(updateMethod), '\n']);
        fprintf(['converge method: ', num2str(convergeMethod),'\n']);
        fprintf(['Kce: ', num2str(Kce), '   rho: ', num2str(rho), '  eps: ', num2str(eps), '\n']);
        
        % iter is used to ensure CE does not run more than max_iter times
        iter = 0;
        max_iter = 10;
        % determine the direction of bias
        if (x0(re_index) > re_val)
            re_type = 1;
            x_ext = ones(1,Kce) * x0(re_index);
        else
            re_type = 2;
            x_ext = zeros(1,Kce);
        end
        
        % formatted messages to print out
        m1 = 'counter: %d, old gamma 1: %g  old gamma 2: %g,  new gamma 1: %g  new gamma 2: %g \n';
        m2 = 'k1: %g  new k2: %g \n';
        m3 = 'relative error: %g   eps: %f \n';
        
        % start multilevel ce
        while(iter < max_iter)
            % CE iteration 1
            
            iter = iter + 1;
            fprintf('\niter %d \nk 1: %f  k 2: %f \ngamma 1: %f  gamma 2: %f \n', iter, k(1), k(2), gamma(1), gamma(2));
            
            counter = 0;
            denom = 0;
            num = 0;
            
            for i=1:Kce
                if (mod(i,Kce/10)==0)
                    fprintf('i: %d\n',i);
                end
                
                [localMax, num_out, denom_out, counter_out] = singleStep(ic, k, gamma, re_val);
                num = num + num_out;
                denom = denom + denom_out;
                counter = counter + counter_out;
                x_ext(i) = localMax;
            end
            
            int_rare = findIntermediateRareEvent(x_ext, floor(Kce*rho), re_type, re_val);
            
            if (int_rare == re_val)
                fprintf('First stage reached the rare event. Update rates... \n');
                old_gamma = gamma;
                gamma = num ./ denom;
                [rel_error num_data] = convergenceTest(gamma, k, fixed_index, eps, counter, ceil(Kce*rho));
                
                fprintf(m1, counter, old_gamma(1), old_gamma(2), gamma(1), gamma(2));
                fprintf(m2, k(1), k(2));
                
                if (convergeMethod == 1)
                    if (rel_error && num_data) % passed convergence test and has enough data
                        fprintf(m3, abs(gamma(2) - 1/k(2)) / gamma(2), eps);
                        fprintf(m1, counter, old_gamma(1), old_gamma(2), gamma(1), gamma(2));
                        fprintf('final k1: %g  k2: %g \n', k(1), k(2));
                        
                        gammas(:, um, cm) = gamma;
                        iter = max_iter;
                        t_tot = toc
                        %                         diary off
                        break;
                    end
                    
                    if (rel_error && ~num_data)
                        fprintf('Rare event is reached within eps tol but not enough counter.\nUpdate and keep running the second CE iteration\n');
                    else if (~rel_error)
                            fprintf('Rare event is reached but error is too large. Setting k according to the update method and keep running the second CE iteration \n');
                        end
                    end
                    
                else
                    if (num_data)
                        fprintf('Rare event is reached. Moving to the convergence routine\n');
                        break;
                    else
                        fprintf('Reached rare event but not enough counter. Update and keep running the second CE iteration.\n');
                    end
                end
            end
            
            fprintf('Intermediate rare event: %d \n', int_rare);
            
            % CE iteration 2
            counter = 0;
            denom = 0;
            num = 0;
            iter2 = 0;
            
            while (iter2 < max_iter)
                iter2 = iter2 + 1;
                fprintf('iter %d - %d\n', iter, iter2);
                
                for i=1:Kce
                    if (mod(i,10000)==0)
                        fprintf('i: %d \n',i);
                    end
                    
                    [localMax, num_out, denom_out, counter_out] = singleStep(ic, k, gamma, int_rare);
                    num = num + num_out;
                    denom = denom + denom_out;
                    counter = counter + counter_out;
                end
                
                old_gamma = gamma;
                gamma = num ./ denom;
                [rel_error num_data] = convergenceTest(gamma, k, fixed_index, eps, counter, ceil(Kce*rho));
                
                switch convergeMethod
                    case 1
                        if (int_rare == re_val)
                            fprintf('Second stage reached the intermediate rare event. Update rates... \n');
                            if (rel_error && num_data) % passed convergence test and has enough data
                                fprintf(m3, abs(gamma(2) - 1/k(2)) / gamma(2), eps);
                                fprintf(m1, counter, old_gamma(1), old_gamma(2), gamma(1), gamma(2));
                                fprintf('final k1: %g  k2: %g \n', k(1), k(2));
                                
                                gammas(:, um, cm) = gamma;
                                iter = max_iter;
                                iter2 = max_iter;
                                t_tot = toc
                                %                                 diary off
                                break
                            end
                            
                            fprintf(m3, abs(gamma(2) - 1/k(2)) / gamma(2), eps);
                            if (rel_error && ~num_data)
                                fprintf('Eps test passed but not enough counter. Update k and keep running the second CE iteration\n');
                            else if (~rel_error)
                                    fprintf('Error is too large. Setting k according to the update method and keep running the second CE iteration \n');
                                end
                            end
                            
                            if (mod(updateMethod,2)) % method = 1 or 3
                                [k, gamma] = updateRates(k, k_original, fixed_index, gamma, updateMethod);
                            else
                                [k, gamma] = updateRates(k, k_original, fixed_index, gamma, updateMethod, index_of_coupdate, old_gamma);
                            end
                            fprintf(m1, counter, old_gamma(1), old_gamma(2), gamma(1), gamma(2));
                            fprintf(m2, k(1), k(2));
                        else
                            if (rel_error && num_data) % passed convergence test and has enough data
                                fprintf('Converged within epsilon! Update and move on to the next intermediate rare event \n');
                                fprintf('relative error: %g   eps: %f \n', abs(gamma(2) - 1/k(2)) / gamma(2), eps);
                                fprintf(m1, counter, old_gamma(1), old_gamma(2), gamma(1), gamma(2));
                                fprintf(m2, k(1), k(2));
                                
                                break;
                            end
                            
                            fprintf(m3, abs(gamma(2) - 1/k(2)) / gamma(2), eps);
                            if (rel_error && ~num_data)
                                fprintf('Eps test passed but not enough counter. Update k and keep running the second CE iteration\n');
                            else if (~rel_error)
                                    fprintf('Error is too large. Setting k according to the update method and keep running the second CE iteration \n');
                                end
                            end
                            
                            if (mod(updateMethod,2)) % method = 1 or 3
                                [k, gamma] = updateRates(k, k_original, fixed_index, gamma, updateMethod);
                            else
                                [k, gamma] = updateRates(k, k_original, fixed_index, gamma, updateMethod, index_of_coupdate, old_gamma);
                            end
                            fprintf(m1, counter, old_gamma(1), old_gamma(2), gamma(1), gamma(2));
                            fprintf(m2, k(1), k(2));
                        end
                    case 2
                        if (int_rare == re_val)
                            if (num_data)
                                fprintf('Rare event is reached. Moving to the convergence routine\n');
                                iter = max_iter; % get out of the outer while loop
                                break; % get out of the inner while loop
                            else
                                fprintf('Reached rare event but not enough counter. Update and go back to first CE iteration.\n');
                            end
                        else
                            fprintf(m1, counter, old_gamma(1), old_gamma(2), gamma(1), gamma(2));
                            break;
                        end
                end
                
            end
        end
        
        if (cm == 2)
            % convergence routine, only for convergenceMethod = 2
            iter = 0;
            fprintf('\n\nstarting convergence sequence...\n');
            if (mod(updateMethod,2)) % method = 1 or 3
                [k, gamma] = updateRates(k, k_original, fixed_index, gamma, updateMethod);
            else
                [k, gamma] = updateRates(k, k_original, fixed_index, gamma, updateMethod, index_of_coupdate, old_gamma);
            end
            fprintf(m1, counter, old_gamma(1), old_gamma(2), gamma(1), gamma(2));
            fprintf(m2, k(1), k(2));
            
            while(iter < max_iter)
                % CE iteration 1
                
                iter = iter + 1;
                fprintf('\niter %d \nk 1: %f  k 2: %f \ngamma 1: %f  gamma 2: %f \n', iter, k(1), k(2), gamma(1), gamma(2));
                
                counter = 0;
                denom = 0;
                num = 0;
                
                for i=1:Kce
                    if (mod(i,10000)==0)
                        fprintf('i: %d\n',i);
                    end
                    
                    [localMax, num_out, denom_out, counter_out] = singleStep(ic, k, gamma, re_val);
                    num = num + num_out;
                    denom = denom + denom_out;
                    counter = counter + counter_out;
                    x_ext(i) = localMax;
                end
                
                int_rare = findIntermediateRareEvent(x_ext, floor(Kce*rho), re_type, re_val);
                
                if (int_rare == re_val)
                    fprintf('First stage reached the rare event. Update rates... \n');
                    old_gamma = gamma;
                    gamma = num ./ denom;
                    
                    [rel_error num_data] = convergenceTest(gamma, k, fixed_index, eps, counter, ceil(Kce*rho));
                    
                    fprintf(m1, counter, old_gamma(1), old_gamma(2), gamma(1), gamma(2));
                    fprintf(m2, k(1), k(2));
                    
                    if (rel_error && num_data) % passed convergence test and has enough data
                        fprintf(m3, abs(gamma(2) - 1/k(2)) / gamma(2), eps);
                        fprintf(m1, counter, old_gamma(1), old_gamma(2));
                        fprintf('final k1: %g  k2: %g \n', k(1), k(2));
                        
                        gammas(:, um, cm) = gamma;
                        iter = max_iter;
                        t_tot = toc
                        %                     diary off
                        break;
                    end
                    
                    fprintf('relative error: %g   eps: %f \n', abs(gamma(2) - 1/k(2)) / gamma(2), eps);
                    if (rel_error && ~num_data)
                        fprintf('Rare event is reached within eps tol but not enough counter.\nUpdate and keep running the second CE iteration\n');
                    else if (~rel_error)
                            fprintf('Rare event is reached but error is too large. Setting k according to the update method and keep running the second CE iteration \n');
                        end
                    end
                    if (mod(updateMethod,2)) % method = 1 or 3
                        [k, gamma] = updateRates(k, k_original, fixed_index, gamma, updateMethod);
                    else
                        [k, gamma] = updateRates(k, k_original, fixed_index, gamma, updateMethod, index_of_coupdate, old_gamma);
                    end
                    fprintf(m1, counter, old_gamma(1), old_gamma(2), gamma(1), gamma(2));
                    fprintf(m2, k(1), k(2));
                end
                
                fprintf('Intermediate rare event: %d \n', int_rare);
                
                % CE iteration 2
                counter = 0;
                denom = 0;
                num = 0;
                iter2 = 0;
                
                while (iter2 < max_iter)
                    iter2 = iter2 + 1;
                    fprintf('iter %d - %d\n', iter, iter2);
                    
                    for i=1:Kce
                        if (mod(i,10000)==0)
                            fprintf('i: %d \n',i);
                        end
                        
                        [localMax, num_out, denom_out, counter_out] = singleStep(ic, k, gamma, int_rare);
                        num = num + num_out;
                        denom = denom + denom_out;
                        counter = counter + counter_out;
                    end
                    
                    old_gamma = gamma;
                    gamma = num ./ denom;
                    [rel_error num_data] = convergenceTest(gamma, k, fixed_index, eps, counter, ceil(Kce*rho));
                    
                    
                    if (int_rare == re_val)
                        fprintf('Second stage reached the rare event. Update rates... \n');
                        counter
                        rel_error
                        num_data
                        if (rel_error && num_data) % passed convergence test and has enough data
                            fprintf(m3, abs(gamma(2) - 1/k(2)) / gamma(2), eps);
                            fprintf(m1, counter, old_gamma(1), old_gamma(2), gamma(1), gamma(2));
                            fprintf('final k1: %g  k2: %g \n', k(1), k(2));
                            
                            gammas(:, um, cm) = gamma;
                            iter = max_iter;
                            iter2 = max_iter;
                            t_tot = toc
                            break;
                            %                         diary off
                            %                         return
                            
                        end
                    end
                    
                    if (rel_error && num_data) % passed convergence test and has enough data
                        fprintf('Converged within epsilon! Update and move on to the next intermediate rare event \n');
                        fprintf('relative error: %g   eps: %f \n', abs(gamma(2) - 1/k(2)) / gamma(2), eps);
                        fprintf(m1, counter, old_gamma(1), old_gamma(2), gamma(1), gamma(2));
                        fprintf(m2, k(1), k(2));
                        break;
                    end
                    
                    fprintf(m3, abs(gamma(2) - 1/k(2)) / gamma(2), eps);
                    if (rel_error && ~num_data)
                        fprintf('Eps test passed but not enough counter. Update k and keep running the second CE iteration\n');
                    else if (~rel_error)
                            fprintf('Error is too large. Setting k according to the update method and keep running the second CE iteration \n');
                        end
                    end
                    
                    if (mod(updateMethod,2)) % method = 1 or 3
                        [k, gamma] = updateRates(k, k_original, fixed_index, gamma, updateMethod);
                    else
                        [k, gamma] = updateRates(k, k_original, fixed_index, gamma, updateMethod, index_of_coupdate, old_gamma);
                    end
                    fprintf(m1, counter, old_gamma(1), old_gamma(2), gamma(1), gamma(2));
                    fprintf(m2, k(1), k(2));
                end
            end
        end
    end
end
matfile = [file_in, '_gammas_eps_', epstr,'_N',num2str(expo)];
save(matfile, 'gammas', 'Kce', 'rho');

