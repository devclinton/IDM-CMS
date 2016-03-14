function [t, p] = migration()
    figure;

    duration = 1024;
    samples  = 1024;
    
    [ t, p ] = ode45(@pdot, linspace(0,duration,samples), InitialPopulation());
    plot(t, p); title('Migration (ODE)');
    
    % Write results to CSV file
    fid = fopen('migration_baseline.csv', 'w');
    try
        [rows cols] = size(p); %#ok<NASGU>
        fprintf(fid, 'A::S{0}');
        for i = 1:rows
            fprintf(fid, ',%f', p(i, 1));
        end
        fprintf(fid, '\n');
        fprintf(fid, 'A::E{0}');
        for i = 1:rows
            fprintf(fid, ',%f', p(i, 2));
        end
        fprintf(fid, '\n');
        fprintf(fid, 'A::I{0}');
        for i = 1:rows
            fprintf(fid, ',%f', p(i, 3));
        end
        fprintf(fid, '\n');
        fprintf(fid, 'A::R{0}');
        for i = 1:rows
            fprintf(fid, ',%f', p(i, 4));
        end
        fprintf(fid, '\n');
        fprintf(fid, 'B::S{0}');
        for i = 1:rows
            fprintf(fid, ',%f', p(i, 5));
        end
        fprintf(fid, '\n');
        fprintf(fid, 'B::E{0}');
        for i = 1:rows
            fprintf(fid, ',%f', p(i, 6));
        end
        fprintf(fid, '\n');
        fprintf(fid, 'B::I{0}');
        for i = 1:rows
            fprintf(fid, ',%f', p(i, 7));
        end
        fprintf(fid, '\n');
        fprintf(fid, 'B::R{0}');
        for i = 1:rows
            fprintf(fid, ',%f', p(i, 8));
        end
        fprintf(fid, '\n');
    catch %#ok<CTCH>
    end
    fclose(fid);
end

% migration model
%
% (species A::S 990)
% (species A::E)
% (species A::I 10)
% (species A::R)
% 
% (param Ki 0.0005)
% (param Kl 0.2)
% (param Kr (/ 1 7))
% (param Kw (/ 1 135))
% 
% (reaction exposureA   (A::S) (A::E) (* Ki A::S A::I))
% (reaction infectionA  (A::E) (A::I) (* Kl A::E))
% (reaction recoveryA   (A::I) (A::R) (* Kr A::I))
% (reaction waningA     (A::R) (A::S) (* Kw A::R))
% 
% (species B::S 100)
% (species B::E)
% (species B::I)
% (species B::R)
% 
% (reaction exposureB   (B::S) (B::E) (* Ki B::S B::I))
% (reaction infectionB  (B::E) (B::I) (* Kl B::E))
% (reaction recoveryB   (B::I) (B::R) (* Kr B::I))
% (reaction waningB     (B::R) (B::S) (* Kw B::R))
% 
% (param Km 0.01)
% 
% (reaction SA->SB (A::S) (B::S) (* Km A::S))
% (reaction EA->EB (A::E) (B::E) (* Km A::E))
% (reaction IA->IB (A::I) (B::I) (* Km A::I))
% (reaction RA->RB (A::R) (B::R) (* Km A::R))
%                                 
% (reaction SB->SA (B::S) (A::S) (* Km B::S 10))
% (reaction EB->EA (B::E) (A::E) (* Km B::E 10))
% (reaction IB->IA (B::I) (A::I) (* Km B::I 10))
% (reaction RB->RA (B::R) (A::R) (* Km B::R 10))

function pop = InitialPopulation()
    pop = [ 990 0 10 0 100 0 0 0 ];
end

function dp = pdot(~, p)
    dp = zeros(8,1);
    Ki = 0.0005;
    Kl = 0.2;
    Kr = 1 / 7;
    Kw = 1 / 135;
    exposedA    = Ki * p(1) * p(3);
    infectiousA = Kl * p(2);
    recoveredA  = Kr * p(3);
    waningA     = Kw * p(4);
    exposedB    = Ki * p(5) * p(7);
    infectiousB = Kl * p(6);
    recoveredB  = Kr * p(7);
    waningB     = Kw * p(8);
    
    Km = 0.01;
    outboundAS = Km * p(1);
    outboundAE = Km * p(2);
    outboundAI = Km * p(3);
    outboundAR = Km * p(4);
    
    outboundBS = Km * 10 * p(5);
    outboundBE = Km * 10 * p(6);
    outboundBI = Km * 10 * p(7);
    outboundBR = Km * 10 * p(8);
    
    dp(1) = waningA     - exposedA    + outboundBS - outboundAS;
    dp(2) = exposedA    - infectiousA + outboundBE - outboundAE;
    dp(3) = infectiousA - recoveredA  + outboundBI - outboundAI;
    dp(4) = recoveredA  - waningA     + outboundBR - outboundAR;
    dp(5) = waningB     - exposedB    + outboundAS - outboundBS;
    dp(6) = exposedB    - infectiousB + outboundAE - outboundBE;
    dp(7) = infectiousB - recoveredB  + outboundAI - outboundBI;
    dp(8) = recoveredB  - waningB     + outboundAR - outboundBR;
end
