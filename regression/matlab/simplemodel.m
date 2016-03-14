function [t, p] = simplemodel()
    figure;

    duration = 730;
    samples  = 365;
    
    [ t, p ] = ode45(@pdot, linspace(0,duration,samples), InitialPopulation());
    plot(t, p); title('SimpleModel (ODE)');
    
    % Write results to CSV file
    fid = fopen('simplemodel_baseline.csv', 'w');
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

% (species S 990)
% (species E)
% (species I 10)
% (species R)
% 
% (param Ki 0.0005)
% (param Kl 0.2)
% (param Kr 0.143)
% (param Kw 0.0074)
% 
% (reaction exposure   (S I) (E I) (* Ki S I))
% (reaction infection  (E)   (I)   (* Kl E))
% (reaction recovery   (I)   (R)   (* Kr I))
% (reaction waning     (R)   (S)   (* Kw R))

function pop = InitialPopulation()
    pop = [990 0 10 0];
end

function dp = pdot(~, p)
    dp = zeros(4,1);
    ki = 0.0005;
    kl = 0.2;
    kr = 0.143;
    kw = 0.0074
    exposed    = ki * p(1) * p(3);
    infectious = kl * p(2);
    recovered  = kr * p(3);
    waning     = kw * p(4);
    dp(1) = waning     - exposed;
    dp(2) = exposed    - infectious;
    dp(3) = infectious - recovered;
    dp(4) = recovered  - waning;
end
