function [t, p] = seirv_sia()

    duration = 548;
    samples  = 512;

    series = linspace(0, 50, 50*samples/duration);
    [ t1, p1 ] = ode45(@pdot,  series, InitialPopulation());
    series = linspace(0, 30, 30*samples/duration);
    [ t2, p2 ] = ode45(@pdotv, series, p1(length(p1),:));
    series = linspace(0,468,468*samples/duration);
    [ t3, p3 ] = ode45(@pdot,  series, p2(length(p2),:)); 

    t = cat(1,cat(1,t1,t2(2:length(t2),:)),t3(2:length(t3),:));
    p = cat(1,cat(1,p1,p2(2:length(p2),:)),p3(2:length(p3),:));
    
    figure;
    plot(p);
    title('SEIRV+SIA (ODE)');
    
    % Write results to CSV file
    fid = fopen('seirv-sia_baseline.csv', 'w');
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
        fprintf(fid, 'V{0}');
        for i = 1:rows
            fprintf(fid, ',%f', p(i, 5));
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
% (species V)
% 
% (param Ki 0.0005)
% (param Kl 0.2)
% (param Kr 0.143)
% (param Kw 0.0074)
% (param Kv 0)
% 
% (reaction exposure    (S I) (E I) (* Ki S I))
% (reaction infection   (E)   (I)   (* Kl E))
% (reaction recovery    (I)   (R)   (* Kr I))
% (reaction waning      (R)   (S)   (* Kw R))
% (reaction vaccination (S)   (V)   (* Kv S))
% 
% (time-event sia 50.0 ((Kv 0.02)))
% (time-event end 80.0 ((Kv 0)))

function pop = InitialPopulation()
    pop = [990 0 10 0 0];
end

function dp = pdot(~, p)
    dp = zeros(5,1);
    Ki = 0.0005;
    Kl = 0.2;
    Kr = 0.143;
    Kw = 0.0074;
    % Kv = 0;
    exposed    = Ki * p(1) * p(3);
    infectious = Kl * p(2);
    recovered  = Kr * p(3);
    waning     = Kw * p(4);
    vaccinated = 0;
    dp(1) = waning     - exposed;
    dp(2) = exposed    - infectious;
    dp(3) = infectious - recovered;
    dp(4) = recovered  - waning;
    dp(5) = vaccinated;
end

function dp = pdotv(~, p)
    dp = zeros(5,1);
    Ki = 0.0005;
    Kl = 0.2;
    Kr = 0.143;
    Kw = 0.0074;
    Kv = 0.02;
    exposed    = Ki * p(1) * p(3);
    infectious = Kl * p(2);
    recovered  = Kr * p(3);
    waning     = Kw * p(4);
    vaccinated = Kv * p(1);
    dp(1) = waning     - vaccinated - exposed;
    dp(2) = exposed    - infectious;
    dp(3) = infectious - recovered;
    dp(4) = recovered  - waning;
    dp(5) = vaccinated;
end
