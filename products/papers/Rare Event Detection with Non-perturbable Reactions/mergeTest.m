% merging algorithm for bins
clear;
clc;

% case 1 all el >= kmin
testvec = {[20 20 20 30 30 30 40 40 40 50], ...
    % case 2 left ed < kmin
    [15 20 20 30 30 30 40 40 40 50],...
    % case 3 right ed < kmin
    [20 20 20 30 30 30 40 40 40 15],...
    % case 4 mid els < kmin
    [20 10 10 10 20 10 10 10 20 20],...
    % case 5 all el < kmin
    [10 10 10 10 10 10 10 10 10 10],...
    % case 6 sum(all el) < kmin
    [1 1 1 1 1 1 1 1 1 1],...
    % case 7 suboptimal case, test for better algs
    [5 5 5 15 5 5 5 2 2 12]};

% correct solution
ansvec = {{[1 1],[2 2],[3 3],[4 4],[5 5],[6 6],[7 7],[8 8],[9 9],[10 10]},...
    {[1 2],[3 3],[4 4],[5 5],[6 6],[7 7],[8 8],[9 9],[10 10]},...
    {[1 1],[2 2],[3 3],[4 4],[5 5],[6 6],[7 7],[8 8],[9 10]},...
    {[1 1],[2 3],[4 5],[6 7],[8 9],[10 10]},...
    {[1 2],[3 4],[5 6],[7 8],[9 10]}...
    {[1 10]},...
    {[1 4],[5 10]}};

beta_max = length(testvec{1});
kappa_min = 20;
single_index = [];
outvec = merge(testvec, kappa_min, beta_max, single_index);

for j=1:numel(testvec)
    vec = testvec{j}
    outvec{j}
    fprintf('total number of bins after merging: %d\n',numel(outvec{j}));
    for i = 1:numel(outvec{j})
        ai = ansvec{j}{i};
        oi = outvec{j}{i};
        check = setdiff(ai,oi);
        if ~isempty(check)
            disp('wrong answer')
            fprintf(['correct answer: ', num2str(ai), ' alg answer: ', num2str(oi),'\n'])
        else
            fprintf(['bin ', num2str(i),' is correct: [', num2str(ai), '],\tsum: ',num2str(sum(vec(ai(1):ai(2)))),'\n'])
        end
        
    end
end