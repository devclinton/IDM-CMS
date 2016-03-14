clear all, close all, clc

%% Copy files from external source
outputname = ['SmallRuns-' date];
mkdir(outputname);

foldername = uigetdir;
files = dir([foldername '\proc*']);

for ii = 1:length(files)
   copyfile([foldername '\' files(ii).name '\done\*'], outputname);
   files(ii).name 
end


%% Split by characteristics 

MList = [0,1];
KList = [1,2];
PList = {'Endemic', 'Vaccinate'};

for M = MList
    for k = KList
        for P = 1:length(PList)
        outdir = [outputname '\' sprintf('M_%d_k_%d_%s', M, k, PList{P}) '\'];
        mkdir(outdir)
        
        runFiles = dir([outputname '\run*']);
        
        for ii = 1:length(runFiles)
            data = load([outputname '\' runFiles(ii).name '\OutputData.mat']);
            vals = textread([outputname '\' runFiles(ii).name '\runParameters.m'], '%s');
            
            
            
            if (abs(data.M-M)<1e-6)&&(abs(data.k-k)<1e-6)&&(~isempty(findstr(vals{1},PList{P})))
                copyfile([outputname '\' runFiles(ii).name '\*.dat'], outdir)
            end 
        end
        
        end
    end
end
