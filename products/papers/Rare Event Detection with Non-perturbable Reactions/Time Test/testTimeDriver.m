% load and call CEDBIR
clear all
clc

% Test runtime for both BA and SSA using 'basic_SIR_FI2_lambda1' and SEED 1234
addpath('\\file.na.corp.intven.com\user2$\mroh\MATLAB\New algorithm prototypes')

% open input file containing model description
[FileName,PathName] = uigetfile({'*.mat',  'mat files (*.mat)'}, 'Select Matlab model input file','MultiSelect','off');
if(isnumeric(FileName)) % user pressed cancel
    display('User canceled. Exiting...')
    return
end
file_in = textscan(FileName,'%s%*[^mat]','Delimiter','.');
file_in = char(file_in{1});
if isempty(file_in)
    error('Invalid file name');
end
load(fullfile(PathName, FileName));

num_try = 4;
rho_default = 0.005;
Kce_default = 1e5;
expo = log10(Kce_default);
M = length(smat(:,1));
% error tolerance values, can be a single number
eps_ce = .15;
eps_final = 0.1;
len_eps_ce_vec = length(eps_ce);
len_eps_final_vec = length(eps_final);

% extract lambda from the .mat file
lambdastr = textscan(FileName,'%*s%*s%*slambda%s','Delimiter','._');
lambdastr = lambdastr{1}{:}
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
% set random number stream with a specific seed
seed = 123456;
alpha = 1;

% write messages and intermediate states to a diary file
diary off
FileName = [file_in, '_', datestr(now, 'mm-dd-hh-MM'),'_N',num2str(expo),...
    '_seed',num2str(seed),'_epsCE',num2str(eps_ce*100),'_epsFinal',num2str(eps_final*100),'_alpha',num2str(alpha*10)];
status = mkdir(FileName);
if (~status)
    display('Cannot create a directory in the current folder. Exiting...')
    return
end
FileName2 = [file_in, '_N',num2str(expo),'_seed',num2str(seed),'_epsCE',num2str(eps_ce*100),'_epsFinal',num2str(eps_final*100),'_SSA'];
FileName3 = [file_in, '_N',num2str(expo),'_seed',num2str(seed),'_epsCE',num2str(eps_ce*100),'_epsFinal',num2str(eps_final*100),'_BA',num2str(alpha*100)];
PathName = pwd;


diary(fullfile(PathName, FileName, [FileName3, '.txt']));
diary on
fprintf(['model ', file_in, ' with rare event = ', num2str(re_val), '\n']);
fprintf(['random number seed: ',num2str(seed), '\n']);
fprintf(['Kce: ', num2str(Kce_default), '   rho_default: ', num2str(rho_default), '\n']);
fprintf('Using lambda: %g\n', lambda);
bin_index = setdiff(1:M, fixed_index);
irr_index = [];
fprintf(['Reactions ', num2str(bin_index), ' will have state-dependent biasing parameters.\n']);
CEDBIR_v4_2(k_default, Kce_default, re_val, re_type, fixed_index, irr_index, bin_index, rho_default, lambda, ic, eps_ce, eps_final, alpha, PathName, FileName);
diary off
% 	Counter: 732	 k: 0.001      0.1219
% 	Gamma values are: 
% 	i: 1	6.7909      3.5316      2.2752      1.8328      1.7106      1.4853      1.3909      1.3666       1.258      1.4789
% 	i: 2	0.80995
% 	PC values are: 
% 	i: 1	0.063336     0.12667     0.19001     0.25334     0.31668     0.38001     0.44335     0.50668     0.57002
% 	i: 2	1
% 	Converged to the final rare event! Relative error: 0.012639	 eps_final: 0.1
% 	Total time: 1709.56	 Total number of iterations: 11	 Total Kce: 1100000 

diary(fullfile(PathName, FileName, [FileName2, '.txt']));
diary on
fprintf(['model ', file_in, ' with rare event = ', num2str(re_val), '\n']);
fprintf(['random number seed: ',num2str(seed), '\n']);
fprintf(['Kce: ', num2str(Kce_default), '   rho_default: ', num2str(rho_default), '\n']);
fprintf('Using lambda: %g\n', lambda);
bin_index = setdiff(1:M, fixed_index);
irr_index = [];
fprintf(['Reactions ', num2str(bin_index), ' will have state-dependent biasing parameters.\n']);
CEDBIR_v4(k_default, Kce_default, re_val, re_type, fixed_index, irr_index, bin_index, rho_default, lambda, ic, eps_ce, eps_final, PathName, FileName);
diary off
% First stage reached the rare event: 201 
% 	Counter: 1227	 k: 0.001     0.11419
% 	Gamma values are: 
% 	i: 1	5.9986       2.772      2.6364      1.6215      1.5174       1.471      1.5155      1.2882      1.2372      1.3336
% 	i: 2	0.83817
% 	PC values are: 
% 	i: 1	0.066667     0.13333         0.2     0.26667     0.33333         0.4     0.46667     0.53333         0.6
% 	i: 2	1
% 	Converged to the final rare event! Relative error: 0.042909	 eps_final: 0.1
% 	Total time: 2085.3159	 Total number of iterations: 12	Total Kce: 1200000 