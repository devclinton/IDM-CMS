function alpha = mle_skew(data, mu, sigma, alpha)

alpha = fminsearch(@(val) computeLogLike(val, data, mu, sigma),[alpha] );

end


function prob = computeLogLike(alpha, data, mu, sigma)

    delta = alpha/sqrt(1+alpha^2);
    omega = real(sqrt(sigma^2/(1-2*delta^2/pi)));
    xi = mu - omega*delta*sqrt(2/pi);
    prob = -sum(log(sn(data,xi, omega, alpha)));
end