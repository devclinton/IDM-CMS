function microvsmacro

ymat = [];

for ii = 1:1000
    ic = 1+.1*randn;
    
    [t,y] = ode45(@rhs, linspace(0,1,200), ic);
    ymat = [ymat, y(:)];
    
end


macro = mean(ymat,2);
stdvals = std(ymat,0,2);
outdata = [t(:), macro(:), stdvals(:), ymat];

save('-ascii', 'microMacro.dat', 'outdata');
    



end


function dydt = rhs(t,y)

dydt = 20*(sech(2*sin(t)) - y)+10*randn;

end