function [covEst] = ComputeCoverageEstimates(pa)

global Coverage

Coverage = 1;

IC = pa.accessGroupPercent;

Factor = pa.accessGroupfactor;

if strcmp(pa.TypeOfCoverage,'Guillaume')

tspan = [0 1000];

options = odeset('Events',@events);

[t,x] = ode45(@rhs,tspan,IC,options,Factor);
xvec = ones(length(t),1) - (x(:,1)+x(:,2)+x(:,3)+x(:,4));

figure(21)
plot(t,x(:,1),'b',t,x(:,2),'g',t,x(:,3),'k',t,x(:,4),'y','LineWidth',[1.5]), hold on
set(gca,'FontSize',[22],'LineWidth',[1.5])
xlabel('time')
ylabel('proportion')

figure(22)
plot(xvec,(x(1,1) - x(:,1))/x(1,1),'b',xvec,(x(1,2)-x(:,2))/x(1,2),'g',xvec,(x(1,3) - x(:,3))/x(1,3),'k',xvec,(x(1,4) - x(:,4))/x(1,4),'y','LineWidth',[1.5]), hold on

set(gca,'FontSize',[22],'LineWidth',[1.5])
xlabel('Global Coverage')
ylabel('Group Coverage')
axis([0 1 0 1])
axis square

xsave = zeros(size(x));

for j = 1:length(x(1,:))
    xsave(:,j) = (x(1,j) - x(:,j))/x(1,j);
end

covEst = [xvec xsave];

end
%%%%%
% clear x
% clear t
% clear xvec
% IC = pa.accessGroupPercent;
% 
% Factor = pa.accessGroupfactor;
% 
% tspan = [0 1000];
% 
% options = odeset('Events',@events);
% 
% [t,x2] = ode45(@rhs2,tspan,IC,options,Factor);
% xvec = ones(length(t),1) - (x2(:,1)+x2(:,2)+x2(:,3)+x2(:,4));
% 
% figure(23)
% plot(t,x2(:,1),'b',t,x2(:,2),'g',t,x2(:,3),'k',t,x2(:,4),'y','LineWidth',[1.5]), hold on
% set(gca,'FontSize',[22],'LineWidth',[1.5])
% xlabel('time')
% ylabel('proportion')
% 
% figure(24)
% plot(xvec,(x2(1,1) - x2(:,1))/x2(1,1),'--b',xvec,(x2(1,2)-x2(:,2))/x2(1,2),'--g',xvec,(x2(1,3) - x2(:,3))/x2(1,3),'--k',xvec,(x2(1,4) - x2(:,4))/x2(1,4),'--y','LineWidth',[1.5]), hold on
% 
% set(gca,'FontSize',[22],'LineWidth',[1.5])
% xlabel('Global Coverage')
% ylabel('Group Coverage')
% axis([0 1 0 1])
% axis square


%%%%%

if strcmp(pa.TypeOfCoverage,'Standard')
clear x
clear t
clear xvec
IC = pa.accessGroupPercent;

Factor = pa.accessGroupfactor;

xvec = [0 1];

x = [zeros(length(Factor),1) Factor];

covEst = [xvec x(:,:)];
end
% 
% xvec2 = [sum(Factor.*IC) sum(IC(1) + IC(2) + Factor(3)*IC(3)+IC(3)*(Factor(3)/(sum(Factor(2:4))))*.5 ...
%     +Factor(4)*IC(4)+IC(4)*(Factor(4)/(sum(Factor(2:4))))*.5)];
% 
% x5 = [1, 1];
% x6 = [Factor(2), 1];
% x7 = [Factor(3) Factor(3)+(Factor(3)/(sum(Factor(2:4))))*.5];
% x8 = [Factor(4) Factor(4)+(Factor(4)/(sum(Factor(2:4))))*.5];
% 
% What = (1-(Factor(3)+(Factor(3)/(sum(Factor(2:4))))*.5))/((Factor(3)/(sum(Factor(3:4)))));
% xvec3 = [sum(IC(1) + IC(2) + IC(3)*Factor(3)+IC(3)*(Factor(3)/(sum(Factor(2:4))))*.5 ...
%     +IC(4)*Factor(4)+IC(4)*(Factor(4)/(sum(Factor(2:4))))*.5) ...
%     sum(IC(1) + IC(2) + IC(3) ...
%     +IC(4)*Factor(4)+IC(4)*(Factor(4)/(sum(Factor(2:4))))*.5+IC(4)*Factor(4)/sum(Factor(3:4))*What)];
% 
% x9 = [1, 1];
% x10 = [1, 1];
% x11 = [Factor(3)+(Factor(3)/(sum(Factor(2:4))))*.5 1];
% x12 = [Factor(4)+(Factor(4)/(sum(Factor(2:4))))*.5 ...
%     Factor(4)+(Factor(4)/(sum(Factor(2:4))))*.5+Factor(4)/sum(Factor(3:4))*What];
% 
% %What = (1-IC(3)*Factor(3)+IC(3)*(Factor(3)/(sum(Factor(2:4))))*.5)/(IC(3)*(Factor(3)/(sum(Factor(3:4)))));
% xvec4 = [sum(IC(1) + IC(2) + IC(3) ...
%     +IC(4)*Factor(4)+IC(4)*(Factor(4)/(sum(Factor(2:4))))*.5+IC(4)*Factor(4)/sum(Factor(3:4))*What) 1];
% 
% x13 = [1, 1];
% x14 = [1, 1];
% x15 = [1 1];
% x16 = [Factor(4)+(Factor(4)/(sum(Factor(2:4))))*.5+Factor(4)/sum(Factor(3:4))*What 1];


%xvec3 = [sum(Factor.*IC) sum(IC(1)+IC(2)+IC(3)+())];

%x9 = [0, 1];
%x10 = [0, Factor(2)];
%x11 = [0, Factor(3)];
%x12 = [0, Factor(4)];


% figure(26)
% plot(xvec1,x1,'b',xvec1,x2,'g',xvec1,x3,'k',xvec1,x4,'y','LineWidth',[1.5]), hold on
% plot(xvec2,x5,'b',xvec2,x6,'g',xvec2,x7,'k',xvec2,x8,'y','LineWidth',[1.5]), hold on
% plot(xvec3,x9,'b',xvec3,x10,'g',xvec3,x11,'k',xvec3,x12,'y','LineWidth',[1.5]), hold on
% plot(xvec4,x13,'b',xvec4,x14,'g',xvec4,x15,'k',xvec4,x16,'y','LineWidth',[1.5]), hold on
% set(gca,'FontSize',[22],'LineWidth',[1.5])
% xlabel('Global Coverage')
% ylabel('Group Coverage')
% axis square
% 
% figure(27)
% plot(xvec,(x2(1,1) - x2(:,1))/x2(1,1),'--b',xvec,(x2(1,2)-x2(:,2))/x2(1,2),'--g',xvec,(x2(1,3) - x2(:,3))/x2(1,3),'--k',xvec,(x2(1,4) - x2(:,4))/x2(1,4),'--y','LineWidth',[1.5]), hold on
% 
% set(gca,'FontSize',[22],'LineWidth',[1.5])
% xlabel('Global Coverage')
% ylabel('Group Coverage')
% axis([0 1 0 1])
% axis square
% 
% covEst2 = [xvec x(:,:)];





end

function y = rhs(t,x,Factor)

denom = sum(Factor'.*x);

y = - Factor' .* x / denom; 

end

function y = rhs2(t,x,Factor)

denom = sum(Factor);

y = - Factor' .* x / denom; 

end

function [value,isterminal,direction] = events(t,x,Factor)

global Coverage

value      = (1-Coverage) - sum(x);
isterminal = +1;
direction  = +1;


end