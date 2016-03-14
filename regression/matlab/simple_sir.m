function [t, p] = simple_sir()
    figure;

    duration = 365;
    samples  = 365;
    
    [ t, p ] = ode45(@pdot, linspace(0,duration,samples), InitialPopulation());
    plot(t, p); title('SimpleSIR (ODE)');
    
    % Write results to CSV file
    fid = fopen('simple-sir_baseline.csv', 'w');
    try
        [rows cols] = size(p); %#ok<NASGU>
        fprintf(fid, 'S{0}');
        for i = 1:rows
            fprintf(fid, ',%f', p(i,1));
        end
        fprintf(fid, '\n');
        fprintf(fid, 'E{0}');
        for i = 1:rows
            fprintf(fid, ',%f', p(i, 2));
        end
        fprintf(fid, '\n');
        fprintf(fid, 'I{0}');
        for i = 1:rows
            fprintf(fid, ',%f', p(i, 3));
        end
        fprintf(fid, '\n');
        fprintf(fid, 'R{0}');
        for i = 1:rows
            fprintf(fid, ',%f', p(i, 4));
        end
        fprintf(fid, '\n');
    catch %#ok<CTCH>
    end
    fclose(fid);
end

% S = 990;
% E = 0;
% I = 10;
% R = 0;
% 
% ki = 0.0005;
% kl = 0.2;
% kr = 0.075;
% kw = 0.005;
% 
% exposure,S+I->E+I,ki;
% infection,E->I,kl;
% recovery,I->R,kr;
% waning,R->S,kw;

function pop = InitialPopulation()
    pop = [990 0 10 0];
end

function dp = pdot(~, p)
    dp = zeros(4,1);
    ki = 0.0005;
    kl = 0.2;
    kr = 0.075;
    kw = 0.005;
    exposed    = ki * p(1) * p(3);
    infectious = kl * p(2);
    recovered  = kr * p(3);
    waning     = kw * p(4);
    dp(1) = waning     - exposed;
    dp(2) = exposed    - infectious;
    dp(3) = infectious - recovered;
    dp(4) = recovered  - waning;
end
