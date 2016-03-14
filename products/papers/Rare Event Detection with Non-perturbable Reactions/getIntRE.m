% function to update rate

function [localExt] = getIntRE(ic, re_type, Kce, rho)

x0 = ic.x0;
t0 = ic.t0;
tf = ic.tf;
stoch_matrix = ic.stoch_matrix;
re_index = ic.re_index;
k = ic.k_original;

localExtVec = zeros(1, Kce);

for i=1:Kce
    x = x0;
    t = t0;
    localExt = re_type * x(re_index);
    
    while(t<tf)
        a = k .* x;
        a0 = sum(a);
        
        % time to the next reaction
        tau = log(1/rand) / a0;
        t = t + tau;
        
        if(t <= tf)
            j = find(cumsum(a)/a0 > rand, 1);
            x = x + stoch_matrix(j,:);
            
            localExt = max(localExt, re_type * x(re_index));
        end
    end
    localExtVec(i) = localExt;
end

localExtVec = sort(localExtVec,'descend');
localExt = localExtVec(ceil(Kce*rho))*re_type;