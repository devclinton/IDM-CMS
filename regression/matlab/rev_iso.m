function [t, p] = rev_iso()
    figure;

    duration = 100;
    samples  = 100;
    
    [ t, p ] = ode45(@pdot, linspace(0,duration,samples), InitialPopulation());
    plot(t, p); title('ODE (MATLAB)');
    
    % Write results to CSV file
    fid = fopen('rev_iso_baseline.csv', 'w');
    try
        [rows cols] = size(p); %#ok<NASGU>
        fprintf(fid, 'A{0}');
        for i = 1:rows
            fprintf(fid, ',%f', p(i,1));
        end
        fprintf(fid, '\n');
        fprintf(fid, 'B{0}');
        for i = 1:rows
            fprintf(fid, ',%f', p(i, 2));
        end
        fprintf(fid, '\n');
    catch %#ok<CTCH>
    end
    fclose(fid);
end

% (species A 100)
% (species B)
% 
% (param k1 0.033)
% (param k2 0.05)
% 
% (reaction fwd (A) (B) (* k1 A))
% (reaction bak (B) (A) (* k2 B))

function pop = InitialPopulation()
    pop = [100 0];
end

function dp = pdot(~, p)
    dp = zeros(2,1);
    k1 = 0.033;
    k2 = 0.05;
    AtoB = k1 * p(1);
    BtoA = k2 * p(2);
    dp(1) = BtoA - AtoB;
    dp(2) = AtoB - BtoA;
end
