clear all, close all, clc
addpath('../')



load('CovarianceMeanStdEndemic.dat')
load('CovarianceMeanStd.dat')
load('SSAEndemic.dat')
load('SSAEradicate.dat')

ThreePanelComparison(SSAEndemic, CovarianceMeanStdEndemic, SSAEradicate, CovarianceMeanStd)
print -depsc2 -painters CovMeanStdPlot.eps
dos('epstopdf CovMeanStdPlot.eps')