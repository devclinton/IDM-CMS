function handle = ThreePanelComparison(TrueEndemic, ComputedEndemic, TrueErad,...
    ComputedErad)

close all
rat = (1+sqrt(5))/2;
handle = figure(1);
set(handle,'paperpositionmode', 'auto','units', 'inches')
set(handle','position',[0 0 6*rat,2]);

subplot(1,3,1)
errorbar(TrueEndemic(:,1), TrueEndemic(:,6), TrueEndemic(:,16),'b');
hold all
errorbar(ComputedEndemic(:,1), ComputedEndemic(:,6), ComputedEndemic(:,16),'r');
plot(TrueEndemic(:,1), TrueEndemic(:,6), 'b', 'linewidth', 2);
plot(ComputedEndemic(:,1), ComputedEndemic(:,6), 'r--', 'linewidth',2);
xlabel('Time [years]')
ylabel('W_{21}')
axis tight
xlim([0 10])

subplot(1,3,2)
errorbar(TrueErad(:,1), TrueErad(:,6), TrueErad(:,16),'b');
hold all
errorbar(ComputedErad(:,1), ComputedErad(:,6), ComputedErad(:,16),'r');
plot(TrueErad(:,1), TrueErad(:,6), 'b', 'linewidth', 2);
plot(ComputedErad(:,1), ComputedErad(:,6), 'r--', 'linewidth',2);
xlabel('Time [years]')
ylabel('W_{21}')
axis tight
xlim([0 10])

subplot(1,3,3)
plot(TrueErad(:,1), TrueErad(:,end),'b','linewidth', 2);
hold all
plot(ComputedErad(:,1), ComputedErad(:,end), 'r--', 'linewidth', 2);
xlabel('Time [years]')
ylabel('P(Eradication)')
axis tight
xlim([0 10])








end