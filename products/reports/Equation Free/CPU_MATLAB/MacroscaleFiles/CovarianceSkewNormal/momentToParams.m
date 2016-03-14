function [xi, omega, alpha] = momentToParams(mn, st, sk)

if abs(sk)>1
    warning('Magnitude of computed skewness parameter %f > 1.  Method of moments may produce poor results',...
        abs(sk));
    sk = sign(sk)*min(.99,abs(sk));  % bound the skewness (otherwise imaginary numbers may results)
end
    
delta = sign(sk)*sqrt((pi/2)*(abs(sk)^(2/3)/(abs(sk)^(2/3)+((4-pi)/2)^(2/3))));
alpha = delta/sqrt(1-delta^2);
omega = sqrt(st^2/(1-2*delta^2/pi));
xi = mn - sqrt(2/pi)*delta*omega;



end