function [gammas, indices] = findCorrelatedIndices(ic, int_rare, re_type, Kce, rho)

seed = 10000*rand;
s = RandStream('mt19937ar','Seed',seed);
RandStream.setGlobalStream(s);

% run the presimulation to compute indices of coupdate
x0 = ic.x0;
t0 = ic.t0;
tf = ic.tf;
stoch_matrix = ic.stoch_matrix;
re_index = ic.re_index;
k = ic.k_original;

% make sure the dimension is correct
M = length(stoch_matrix(:,1));
gammas = zeros(2,M);

for iter=1:2
    denom = zeros(1,M);
    num = zeros(1,M);

    for i=1:Kce
        if (mod(i,Kce/10)==0)
            fprintf('i: %d\n',i);
        end
        
        n = zeros(1,M);
        lambda = zeros(1,M);
        
        x = x0;
        t = t0;
        while (t<tf)
            
            if (x(re_index) == int_rare)
                num = num + n;
                denom = denom + lambda;
                break;
            end
            
            a = k .* x;
            a0 = sum(a);
            tau = log(1/rand) / a0;
            t = t + tau;
            j = find(cumsum(a)/a0 > rand, 1);
            
            if t<tf
                n(j) = n(j) + 1;
                x = x + stoch_matrix(j,:);
            else
                tau = tf - (t-tau);
            end
            
            lambda = lambda + a*tau;
        end
    end
    
    gammas(iter,:) = num./denom;
end

% this ratio should be updated for more than 2 reactions
ratios = gammas(:,1)./gammas(:,2)
abs(ratios(1)-ratios(2))/ratios(1)
if (abs(ratios(1)-ratios(2))/ratios(1) < 0.1)
    disp('passed')
end

% indices = 1;

