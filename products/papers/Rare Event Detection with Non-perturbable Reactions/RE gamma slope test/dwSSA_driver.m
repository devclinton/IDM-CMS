clear all 
clc

addpath(genpath('\\file.na.corp.intven.com\user2$\mroh\MATLAB\New algorithm prototypes'))

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
rho_default = 0.01;
Kce_default = 1e5;
expo = log10(Kce_default);
M = length(smat(:,1));

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
PathName = cd;
FileName2 = [file_in, '_', datestr(now, 'mm-dd-hh-MM'),'_N',num2str(expo),'_rho',num2str(rho_default),'_seed',num2str(seed)];
status = mkdir(FileName2);
if (~status)
    display('Cannot create a directory in the current folder. Exiting...')
    return
end
diary(fullfile(PathName,FileName2,[FileName2, '.txt']));
diary on
fprintf(['model ', file_in, ' with rare event = ', num2str(re_val), '\n']);
fprintf(['random number seed: ',num2str(seed), '\n']);
fprintf(['Kce: ', num2str(Kce_default), '   rho_default: ', num2str(rho_default), '\n']);

target = .5;
abs_tol = .05;
inc = 2;
for i=-10:inc:10
% for i = 0:0
    re_val_test = re_val + i;
    fprintf('\n\ni: %d \t re_val used: %d \n', i, re_val_test);
    dwSSA(k_default, Kce_default, re_val_test, re_type, rho_default, ic, target, abs_tol, PathName, FileName2);
end

datestr(now, 'mm-dd-hh-MM')
diary off




