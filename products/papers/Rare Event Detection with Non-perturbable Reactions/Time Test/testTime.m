% for comparing time between ssa and basil's alg
clc;
clear;
addpath('\\file.na.corp.intven.com\user2$\mroh\MATLAB\New algorithm prototypes')

% set random number stream with a specific seed
seed = 200;
load('basic_SIR_FI2_lambda1.mat');
Kce = 100000;
M = 2;
bin_index = 1;
irr_index = [];

if (x0(re_index) > re_val)
    re_type = -1;
else
    re_type = 1;
end

[gamma prop_cutoff, num, denom]  = initializeCEvars_v4(M, 10, fixed_index, irr_index, bin_index);

x_extSSA = zeros(1,Kce);
x_extBA = zeros(1,Kce);
maxRP = zeros(M, Kce);
minRP = zeros(M, Kce);

tic;
parfor i=1:Kce
    if (mod(i,Kce/10)==0)
        fprintf('i: %d\n',i);
    end
    [localMax, num_out, denom_out, counter_out, minRelProp, maxRelProp]  = solveOnce_v4(ic, k, gamma, prop_cutoff, bin_index, [fixed_index irr_index], re_val, re_type);
    
    if counter_out
        counter = counter + counter_out;
    end
    x_extSSA(i) = localMax;
    maxRP(:,i) = maxRelProp';
    minRP(:,i) = minRelProp';
end
tssa = toc % 704.2718

int_rareSSA = findIntermediateRareEvent(x_extSSA, floor(Kce*rho), re_type, re_val) %174

figure1 = figure(1);
hist(x_extSSA,50)
title('100K SSA IRE','FontWeight','demi');
xlabel('x closest to RE','FontWeight','demi')
ylabel('frequency','FontWeight','demi')
annotation(figure1,'textbox',... 
    [0.603571428571428 0.802380952380952 0.228571428571429 0.0785714285714303],...
    'String',{['tot time: ', num2str(tssa)]},...
    'FitBoxToText','off', 'LineStyle','none');
saveas(gcf, 'SSA.jpg')

tic;
parfor i=1:Kce
    if (mod(i,Kce/10)==0)
        fprintf('i: %d\n',i);
    end
    [localMax, num_out, denom_out, counter_out, minRelProp, maxRelProp]  = solveOnce_v4_2(ic, k, gamma, prop_cutoff, bin_index, [fixed_index irr_index], re_val, re_type);
    
    if counter_out
        counter = counter + counter_out;
    end
    x_extBA(i) = localMax;
    maxRP(:,i) = maxRelProp';
    minRP(:,i) = minRelProp';
end
tba = toc  %  577.1084
int_rareBA = findIntermediateRareEvent(x_extBA, floor(Kce*rho), re_type, re_val) %173

figure2 = figure(2);
hist(x_extBA,50)
title('100K BA IRE','FontWeight','demi');
xlabel('x closest to RE','FontWeight','demi')
ylabel('frequency','FontWeight','demi')
annotation(figure2,'textbox',...
    [0.603571428571428 0.802380952380952 0.228571428571429 0.0785714285714303],...
    'String',{['tot time: ', num2str(tba)]},...
    'FitBoxToText','off', 'LineStyle','none');
saveas(gcf, 'BA.jpg')

figure(3)
[n1, x1] = hist(x_extSSA,50);
hist(x_extSSA,x1);
h = findobj(gca,'Type','patch');
set(h,'FaceColor','r','EdgeColor','w','facealpha',0.65)
hold on
n2 = hist(x_extBA,x1); 
hist(x_extBA,x1);
h = findobj(gca,'Type','patch');
set(h,'facealpha',0.65);
xlabel('x closest to RE','FontWeight','demi')
ylabel('frequency','FontWeight','demi')
title('100000 Kce IRE Simulations', 'FontWeight','demi');
legend('SSA','BA');
hold off
% normalize
x_extSSAn= x_extSSA/sum(x_extSSA);
x_extBAn= x_extBA/sum(x_extBA);

v = axis;
xpos = v(1)+(v(2)-v(1))*.1;
ypos = max(max(n1),max(n2))+(v(4)-max(max(n1),max(n2)))*.5;
euclidean_distance = sqrt(sum(abs(x_extSSAn-x_extBAn).^2));
manhattan_distance = sum(abs(x_extSSAn-x_extBAn));
tstr = {['Euclidian d=',num2str(euclidean_distance,'%2.3f'),'  Manhattan d=',num2str(manhattan_distance,'%2.3f')]};
text(xpos, ypos, tstr);
saveas(gcf, 'hist_distance_SSA_BA.jpg')
% delta = 0;
% for i=1:Kce
%     
% end
save('timeTestRestult.mat', 'tssa', 'tba', 'int_rareSSA', 'int_rareBA', 'x_extSSA', 'x_extBA', 'euclidean_distance','manhattan_distance');
