clear all
close all

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%%%RunModel%%%

%Parameters
param.beta=.5;
param.alpha=0.2;
param.N=10000;
param.mu=0.02;
param.gamma=0.1;
param.nu=0.02;

n=5;
param.n=n;

%%%% IC %%%%

IC=round([param.N/n/3*ones(3*n,1)]); %;zeros(n,1);zeros(n,1)];

CreateCMDLFile2(param,IC);

%%%% Model File Designation and config file parameters %%%%

model = 'exp.cmdl';
directory = '.\';

Co.solver = 'TAU';
Co.duration = '20';
Co.runs = '5';
Co.samples = '10';
Co.epsilon = '.01';
Co.Nc = '2';
Co.Multiple = '10';
Co.SSAruns = '100';

CreateConfigFile(Co);

cmd = ['C:/SRC/CMS/trunk/framework/compartments/bin/x64/debug/compartments.exe -model ', model,' -directory ', directory, ' -config config.json'];

system(cmd)

M = fileread('trajectories.json');
F = parse_json(M);


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%%%Plotting%%%

%%% Find Age Distribution
jjj=0;

PlotAgeGroups

%%%Compare to SSA simulations %%%
    
Co.solver = 'SSA';

CreateConfigFile(Co);

cmd = ['C:/SRC/CMS/trunk/framework/compartments/bin/x64/debug/compartments.exe -model ', model,' -directory ', directory, ' -config config.json'];

system(cmd)

M = fileread('trajectories.json');
F = parse_json(M);

%%%Plotting%%%

%%% Find Age Distribution
jjj=1;

PlotAgeGroups