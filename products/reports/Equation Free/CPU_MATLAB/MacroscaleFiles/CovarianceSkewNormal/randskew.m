function out = randskew(N, xi, omega, alpha)

delta = alpha/sqrt(1+alpha^2);
u0 = randn(N,1); v = randn(N,1);
u1 = delta*u0+sqrt(1-delta^2)*v;
out = xi+omega*sign(u0).*u1;



end