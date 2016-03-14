clear all, close all, clc

%% directories and commands
procs = 4;
queueDir = '../RunQueue/';
skip = [];
%% Parameter List 
requiredList = {};
requiredList{1} = {'../CMSFiles/'};

buildList = {};
buildList{1} = {{'Endemic Case','Vaccinated Case'},...
    {'../ModelFiles/paramsEndemic/','../ModelFiles/paramsVaccinate/'}};
buildList{2} = {{'Forward Euler', 'Linear Fit'}, {'../TimeStepper/ForwardEuler/','../TimeStepper/LinearFit/'}};
buildList{2} = {{'Linear Fit'}, {'../TimeStepper/ForwardEuler/'}};

buildList{3} = {{'SSA', 'Only Mean', 'Covariance', ...
    'Covariance with mean readjustment', 'Covariance with direct eradication', 'Mean and Std', 'Lognormal',...
    'DisreteICDF', 'Discrete Covariance', 'Covariance Mean Std', 'Covariance Reduced','Covariance Skew'},...
    {'../MacroscaleFiles/MacroSSA/', '../MacroscaleFiles/Mean/',...
    '../MacroscaleFiles/CovariancePolynomial/', '../MacroscaleFiles/CovarianceStabilized/', ...
    '../MacroscaleFiles/CovarianceRecompute/', '../MacroscaleFiles/MeanStd/', ...
    '../MacroscaleFiles/LogNormal/', '../MacroscaleFiles/Discrete/',...
    '../MacroscaleFiles/CovarianceDiscrete/','../MacroscaleFiles/CovarianceNormal/',...
    '../MacroscaleFiles/CovarianceReduce/', '../MacroscaleFiles/CovarianceSkewNormal/'}};
buildList{3} = {{'Only Mean'},...
    {'../MacroscaleFiles/Mean/'}};
%varList = struct('names', {'k', 'M',
varList = {...
            {'dt', {1/12}}, ...
            {'NRuns', {1000}},...
            {'k', {2}}, ...
            {'M', {0}}};
 

 dof = 1;
 for ii = 1:length(buildList)
     dof = dof*length(buildList{ii}{1});
 end
 for ii = 1:length(varList)
     dof = dof*length(varList{ii}{2});
 end 
 disp(sprintf('Generating %d configurations over %d processors', dof, procs));
 
 %% Make directories and copy files
 for ii = 1:procs
     mkdir(sprintf('%s/proc_%d/',queueDir,ii));
 end
 
%% Generate Params Strings
DofList = generateParamString(varList, buildList);

fidlist = {};

for ii = 1:procs
   fidlist{ii} = fopen(sprintf('%s/proc_%d/processQueue.m', queueDir, ii),'wt');
   mkdir(sprintf('%s/proc_%d/done/', queueDir, ii));
end



for ii = 1:length(DofList)
    
    if any(ii==skip)
        continue
    end
    
   runsPerProc = floor(length(DofList)/procs);
 %  currProc = ceil(ii/runsPerProc); % evenly split load for now
   currProc = mod(ii, procs)+1;
   currRun = ii;
   
   if (currProc > procs)
       currProc = procs; 
   end
   
   currDir = sprintf('%s/proc_%d/run_%d/',queueDir,currProc, currRun);
   

   
   fprintf(fidlist{currProc}, sprintf('cd(''run_%d'');\n', currRun));
   fprintf(fidlist{currProc}, sprintf('try\n'));
   fprintf(fidlist{currProc}, sprintf('executeRun;\n'));
   % Make all directories 
   mkdir(currDir);
   
   % Copy required files
   for jj = 1:length(requiredList)
       copyfile([requiredList{jj}{1} '*'], currDir);
   end   
   % Copy needed files from build list
   for jj = 1:length(buildList)
       copyfile([DofList{ii}{jj} '*'], currDir);
   end
   
   fid = fopen(sprintf('%s/runParameters.m',currDir), 'wt');
   
   for jj = 1:length(buildList)
      fprintf(fid, '''%s'';\n', DofList{ii}{jj}); 
   end
   for jj = 1:length(varList)
       fprintf(fid, '%s = %e;\n', varList{jj}{1}, DofList{ii}{jj+length(buildList)});  
   end

   
   fclose(fid);
  
   fprintf(fidlist{currProc},sprintf('cd(''..'');\n'));
   fprintf(fidlist{currProc},sprintf('movefile(''run_%d'', ''done'')\n', currRun));
   fprintf(fidlist{currProc},sprintf('catch\n \tdisp(''Error in run %d'');\n cd(''..'');\n end\n\n', currRun));
   
end

fclose all

 
 
 

