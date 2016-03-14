% reversible isomerization model definition
clear all
k_default = [0.12  1];
x0= [100 0];
smat = [-1 1; 1 -1];
rmat = [1 0; 0 1];
t0 = 0;
tf = 10;
re_val = 30;
re_index = 2;

% fixed index are the reaction index (indicies) that will be set to 1 upon completion of the convergence routine
fixed_index = 2;

rho = 0.005;
lambdas = 0.2:0.2:1;
for i=1:length(lambdas)
    
    lambda = lambdas(i);
    lambdastr = textscan(num2str(lambda),'%s%s','Delimiter','.');
    lambdastr = strcat(char(lambdastr{1}),char(lambdastr{2}));
    fname = ['reversible_isom_lambda', lambdastr,'.mat']
    % structure for single step method
    ic = struct('x0', x0, 't0', t0, 'tf', tf, 'stoch_matrix', smat, 'reac_matrix', rmat, 're_index', re_index, 'k_original', k_default);
    save(fname)
end

