clear all, close all, clc

%% directories and commands
procs = 4;
queueDir = '../RunQueue/';
matlabcommand = ['"' matlabroot '\bin\matlab"'];

% farm off other runs to other procs
here = pwd;
for ii = 1:procs-1
    cd(sprintf('proc_%d',ii))
    dos([matlabcommand ' -nodesktop -nosplash -minimize -r processQueue'])
    cd(here)
end

% Process final proc in this matlab window
ii = procs;
cd(sprintf('proc_%d',ii));
processQueue;
 
 

