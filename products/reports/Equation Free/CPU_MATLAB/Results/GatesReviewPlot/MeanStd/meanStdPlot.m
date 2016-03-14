clear all, close all, clc
addpath('../')

load('MeanStdOutputErad.dat')
load('MeanStdOutputEndemic.dat')
load('SSAEndemic.dat')
load('SSAEradicate.dat')

ThreePanelComparison(SSAEndemic, MeanStdOutputEndemic, SSAEradicate, MeanStdOutputErad)
print -depsc2 -painters MeanStdPlot.eps
dos('epstopdf MeanStdPlot.eps')