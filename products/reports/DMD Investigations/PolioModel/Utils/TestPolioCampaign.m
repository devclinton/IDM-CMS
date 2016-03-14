function TestPolioCampaign()

    Years = 20;

%   Detailed explanation goes here
    close all
    figure;
    
    HybridSimulations(Years);

    StochasticSimulations('SSA','Polio.emodl',Years);
    
    TimeToEradication();
    
end

function StochasticSimulations(solver, ModelFileName,Duration)

    ModelFile = fileread(ModelFileName);
    WithinTrajSamples = 200;
    NumTrajectories = 200;

    [ ~, data ] = RunModel(ModelFile, TestConfig, solver, NumTrajectories, Duration, WithinTrajSamples);

    subplot(1, 2, position); plot(linspace(0,Duration,WithinTrajSamples),data','LineWidth',[1.5]);
    set(gca,'FontSize',[22],'LineWidth',1.5)
    xlabel('time')
    ylabel('Population')
    title(['Stochastic Simulations'])

end

function HybridSimulations(Years)

%Parameters
param.betaW= 104.4457;
param.betaV=2.6111;
param.alpha=0.1;
param.N=100000;
param.mu=0.042;
param.gamma=1/(28/365);
param.nu=0.042;

param.MC = .1;

n=1;
param.n=n;

p=[];

%%%% IC %%%%

IC=floor([param.N/n*ones(n,1);zeros(n,1);zeros(n,1);zeros(n,1);zeros(n,1)]);

SIANum = 4;
RMag = .7;
SIAMag = .7;

StateMapping = [];
tsave = [];
Xfsave = [];

Eventtimes = zeros(SIANum*2+1,1);
index = 1/52;

for j = 1 : (SIANum*2+1)
    Eventtimes(j) = mod(index,1);

    if mod(j,2) == 1
         index = 1/52;
    else
        index =  3/52;
    end
    
    if j == SIANum*2+1
       Eventtimes(j) = 1; 
    end

end

%tspan = 0:.0001:Eventtimes(1);
tspan = [0 Eventtimes(1)];
options = odeset('RelTol',1e-10,'AbsTol',1e-10);
g = RMag;
f = 0;
Index = 1;

for j = 1:length(Eventtimes)*Years
    
[tf,Xf]=ode15s(@rhsaccess,tspan,IC,options,param,p,f,g,Eventtimes);
 
if (mod(Index,length(Eventtimes)) == 0)
     f = 0;
     tspan = [tf(end) tf(end)+Eventtimes(1)];
     StateMapping = [StateMapping Xf(end,:)'];
     Index = 1;
 elseif (mod(Index,length(Eventtimes)) == length(Eventtimes) - 1 )
     f = 0;
     tspan = [tf(end) tf(end)+(1-sum(Eventtimes(1:end-1)))];
     Index = Index + 1;
 elseif (mod(Index,2) == 1) 
     f = SIAMag;
     tspan = [tf(end) tf(end)+Eventtimes(mod(Index+1,length(Eventtimes)))];
     Index = Index + 1;
 elseif (mod(Index,2) == 0)
     f = 0;
     tspan = [tf(end) tf(end)+Eventtimes(mod(Index+1,length(Eventtimes)))];
     Index = Index + 1;     
end
    tsave = [tsave;tf];
    Xfsave = [Xfsave;Xf];
    IC=Xf(end,:);   
end

subplot(1,2,1), 
plot(tsave,Xfsave(:,1),'b',tsave,Xfsave(:,2),'r',tsave,Xfsave(:,3),'g',tsave,Xfsave(:,4),'y',tsave,Xfsave(:,5),'c','LineWidth',[1.5]), hold on
set(gca,'FontSize',[22],'LineWidth',[1.5])
xlabel('time')
ylabel('Population')
axis([0 Years 0 max(max(Xfsave))])
title(['Hybrid ODE'])

end