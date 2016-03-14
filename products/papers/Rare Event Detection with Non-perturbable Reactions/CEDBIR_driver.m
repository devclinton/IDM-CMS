% load and call CEDBIR
clear all 
clc

% lambda controls how aggressive we want to update the biasing parameter
% for the excluded reactions

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
eps_ce_vec = .15 %[.2 .175 .15 0.125 0.1];
eps_final_vec = .075 %[0.1 .075 0.05];
len_eps_ce_vec = length(eps_ce_vec);
len_eps_final_vec = length(eps_final_vec);

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
% set random number stream with a specific seed
seed = ceil(10000*rand);
s = RandStream('mt19937ar','Seed',seed);
RandStream.setGlobalStream(s);

% write messages and intermediate states to a diary file
diary off
FileName2 = [file_in, '_', datestr(now, 'mm-dd-hh-MM'),'_N',num2str(expo),'_seed',num2str(seed)];
status = mkdir(FileName2);
if (~status)
    display('Cannot create a directory in the current folder. Exiting...')
    return
end
diary(fullfile(PathName, FileName2, [FileName2, '.txt']));
diary on
fprintf(['model ', file_in, ' with rare event = ', num2str(re_val), '\n']);
fprintf(['random number seed: ',num2str(seed), '\n']);
fprintf(['Kce: ', num2str(Kce_default), '   rho_default: ', num2str(rho_default), '\n']);
fprintf('Using lambda: %g with CEDBIR VER 2\n', lambda);


%%%%%%%  Determine which reactions are not important (irrelevant),
%%%%%%%  i.e. gamma = 1 at all times
%%%%%%%  if fixed_index is found to be irrelevant, we don't need to run
%%%%%%%  CEDBIR. Normal multilevel CE with gamma_"fixed_index" = 1 will be
%%%%%%%  more efficient
ir_flag = 0;
exit_flag = 0;
bin_index = setdiff(1:M, fixed_index);
irr_index = [1,2,7];
bin_index = setdiff(bin_index, irr_index);
if (ir_flag)
    fprintf('Start detecting irrelevant reactions...\n');
    [irr_index gams exit_flag] = findIrrelevantReactions_v4(ic, Kce_default, rho_default, fixed_index, re_type, re_val, num_try);
    % figure out the reactions to bin
    bin_index = setdiff(bin_index, irr_index);
end
if exit_flag
    diary off;
    delete(fullfile(PathName, FileName2, [FileName2, '.txt']));
    return;
end
fprintf(['Reactions ', num2str(bin_index), ' will have state-dependent biasing parameters.\n']);

% for now
fixed_index = [7];
irr_index = [1,2];
bin_index = setdiff(1:M, irr_index);

alpha = 1;
% CEDBIR_v4_2(k_default, Kce_default, re_val, re_type, fixed_index, irr_index, bin_index, rho_default, lambda, ic, eps_ce_vec, eps_final_vec, alpha, PathName, FileName2);
CEDBIR_v4(k_default, Kce_default, re_val, re_type, fixed_index, irr_index, bin_index, rho_default, lambda, ic, eps_ce_vec, eps_final_vec, PathName, FileName2);
datestr(now, 'mm-dd-hh-MM')
diary off




