clear all, close all, clc
addpath('../')

load('MeanEndemic.dat')
load('MeanEradicate.dat')
load('SSAEndemic.dat')
load('SSAEradicate.dat')

ThreePanelComparison(SSAEndemic, MeanEndemic, SSAEradicate, MeanEradicate)
print -depsc2 -painters MeanPlot.eps
dos('epstopdf MeanPlot.eps')