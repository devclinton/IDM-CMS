% ODE for SIR

function dy = sir_heterogeneous(t, y, beta, b, aj, gamma)

% dylen = length(y);
% dy = zeros(dylen,1);
% dy = [S1 S2 S3 I1 I2 I3 R1 R2 R3]

% beta = [0   b12 b13;
%         b21 0   b23;
%         b31 b32 0]

dy = zeros(9,1);
for i=1:3
    exc_i = find(1:3 ~= i);
    dy(i) = -beta * y(i) * (aj(i)*y(i+3) + b * sum(I(exc_i+3)));
end

% for i=4:6
    dy(4:6) = beta * y(1:3) .* (aj(1:3) .* y(1:3) + b * sum(I(exc_i))) + gamma * y(i);


for i=7:9
    dy(i) = gamma * y(i-3);
end
