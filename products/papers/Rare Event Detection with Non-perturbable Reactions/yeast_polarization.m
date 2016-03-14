% basic SIR model
clear all

SA = 50.2655;
V = .0477;
%    R   L  RL G  Ga Gbg Gd
x0 = [50 2  0  50  0  0  0];
k_default = [0.0796*V, 0.0004, 0.0020/V, 0.0100, 0.0005/V, 0.1000, 50.2655/V, 3.2111];
k = k_default;
%      R   L   RL  G   Ga  Gbg Gd
smat =[1   0   0   0   0   0   0;   % R1
       -1  0   0   0   0   0   0;   % R2
       -1  0   1   0   0   0   0;   % R3
       1   0   -1  0   0   0   0;   % R4
       0   0   -1  -1  1   1   0;   % R5
       0   0   0   0   -1  0   1;   % R6
       0   0   0   1   0   -1  -1;  % R7
       0   0   1   0   0   0   0;]; % R8
       
%      R   L   RL  G   Ga  Gbg Gd
rmat =[0   0   0   0   0   0   0;   % R1
       1   0   0   0   0   0   0;   % R2
       1   1   0   0   0   0   0;   % R3
       0   0   1   0   0   0   0;   % R4
       0   0   1   1   0   0   0;   % R5
       0   0   0   0   1   0   0;   % R6
       0   0   0   0   0   1   1;  % R7
       0   0   0   0   0   0   0;]; % R8
   
t0 = 0;
tf = 5;
re_val = 40;
re_index = 6;

% fixed index are the reaction index (indicies) that will be set to 1 upon completion of the convergence routine
fixed_index = 8;

rho = 0.005;
lambdas = 0.6:0.2:1;
for i=1:length(lambdas)
    lambda = lambdas(i);
    lambdastr = textscan(num2str(lambda),'%s%s','Delimiter','.');
    lambdastr = strcat(char(lambdastr{1}),char(lambdastr{2}));
    fname = ['yeast_polarization_lambda', lambdastr,'.mat'];
    % structure for single step method
    ic = struct('x0', x0, 't0', t0, 'tf', tf, 'stoch_matrix', smat, 'reac_matrix',rmat, 're_index', re_index, 'k_default', k_default);
    save(fname)
end
