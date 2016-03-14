function y = sn(x, xi, omega, alpha)

    pdf = @(x) 1./sqrt(2*pi)*exp(-x.^2/2);
    cdf = @(x) 0.5*(1+erf(x/sqrt(2)));

    xhat = (x-xi)/omega;
    
    y = 2/omega*pdf(xhat).*cdf(alpha*xhat);

end