clc 
clear all
close all

start_dir = '\\file.na.corp.intven.com\user2$\mroh\MATLAB\New Algorithm TFS\RE gamma slope test';
dir_name = uigetdir(start_dir, 'Select the directory containing output files');
if dir_name==0
    error('Output directory has not be specified. Exiting...');
end
file_name_base = dir_name(length(start_dir)+2:end)

re_val = 30;
starti = -10;
endi = 10;
incri = 2;

ireSize = 5; % estimated number of ires from the data
M =  2;
%     for each ire, gam, n, num, denom
ireMat = zeros(1,ireSize);
gammaMat = zeros(M, ireSize);
nMat = zeros(M, ireSize);
numMat = zeros(M, ireSize);
denomMat = zeros(M, ireSize);
counterMat = zeros(1, ireSize);


lenData_old = 0;
for i=starti:incri:endi
    re_val_test = re_val + i;
    file_name = strcat(file_name_base,'_re_val_',num2str(re_val_test),'.mat');
    try
        S = load(fullfile(dir_name,file_name));
    catch err
        error(err.identifier, err.message);
    end
    
    % local vars
    ires = S.ires;
    lenData = length(ires);
    % store into a global table
    sind = lenData_old+1;
    eind = lenData_old + lenData;
    ireMat(sind:eind) = ires;
    counterMat(sind:eind) = S.counters;
    gammaMat(:,sind:eind) = S.gammas;
    nMat(:,sind:eind) = S.ns;
    numMat(:,sind:eind) = S.nums;
    denomMat(:,sind:eind) = S.denoms;
    lenData_old = eind;
end

% sort the matrix according to ire data
[ireMat inds] = sort(ireMat); % sort ascending
gammaMat = gammaMat(:,inds);
nMat = nMat(inds);
numMat = numMat(inds);
denomMat = denomMat(inds);

%%        
figure(1)
subplot(2,1,1), plot(ireMat, gammaMat(1,:),'b.');
xlabel('intermediate rare event');
ylabel('gamma 1');
subplot(2,1,2), plot(ireMat, gammaMat(2,:),'r.');
xlabel('intermediate rare event');
ylabel('gamma 2');

%% calculate the exact answer
X =[100 0];
total_population = sum(X);
k1 = .12;
k2 =  1;
final_time = 10;
lenProb = length(starti:incri:endi);
reProbs = zeros(1, lenProb);
rcounter = 1;
for j = starti:incri:endi
    qSize = re_val + j; %0,1,...,rare_event
    q = zeros(qSize, qSize);
    q(1,1) = -k1 * X(1);
    q(1,2) =  k1 * X(1);
    for i=2:(qSize - 1)
        q(i,i-1) =(i - 1) * k2;
        q(i,i+1) =(total_population - i + 1) * k1;
        q(i,i) = -sum(q(i,:));
    end
    % initial probability distribution
    ip = zeros(qSize,1);
    ip(1) = 1;  % because X(2) at time 0 is 0(index 1) with certainty
    exact_answer = ip'*expm(q * final_time);
    reProbs(rcounter) = exact_answer(end);
    rcounter = rcounter + 1;
end


figure(2)
semilogy(re_val+(starti:incri:endi), reProbs,':^','MarkerFaceColor',[1 0 1],'Marker','^','Color',[1 0.6 0.78]);

xlabel('population of B');
ylabel('prob(X(B)) at t_{final}');
title('Exact probability');

%% for studying correlation between gamma and counter
clc
% close all
inds30 = find(ireMat == 30);
gammas30 = gammaMat(:,inds30);
exactProb30 = exact_answer(31);
counter30 = counterMat(inds30);
probHat = counter30/1e5;
figure(3)
plot(gammas30(1,:),probHat,'d');
hold on
plot(linspace(min(gammas30(1,:)),max(gammas30(1,:)),50), linspace(exactProb30, exactProb30, 50),'r--','LineWidth',1.5)
hold off

%%




