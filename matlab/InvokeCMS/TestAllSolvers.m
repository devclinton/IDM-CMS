function TestAllSolvers()
%TESTALLSOLVERS Summary of this function goes here
%   Detailed explanation goes here

    figure;
    
    % ODE for reference
    [ t, p ] = ode45(@pdot, [0 1000], [990 0 10 0]);
    subplot(2,3,1);
    plot(t, p); title('ODE (MATLAB)');
    
    PlotRun('SSA',   2, 'SSA');
    PlotRun('FIRST', 3, 'First Reaction');
    PlotRun('TAU',   4, 'Tau-Leaping');
    PlotRun('R',     5, 'R-Leaping');
    PlotRun('B',     6, 'B-Leaping');
    
end

function dp = pdot(~, p)
    dp = zeros(4,1);
    dp(1) = (p(4) * 0.0074) - (p(1) * p(3) * 0.0005); % +waning immunity - newly exposed
    dp(2) = (p(1) * p(3) * 0.0005) - (p(2) * 0.2);    % +newly exposed   - now infectious
    dp(3) = (p(2) * 0.2) - (p(3) * 0.143);            % +now infectious  - recovered
    dp(4) = (p(3) * 0.143) - (p(4) * 0.0074);         % +recovered       - waning immunity
end

function PlotRun(solver, position, caption)

tStart = tic;
    [ ~, data ] = RunModel(TestModel, TestConfig, solver, 10, 730, 1000);
tElapsed = toc(tStart);
    fprintf('Run time for %s: %f\n', caption, tElapsed);
    subplot(2, 3, position); plot(data'); title(caption);

end
