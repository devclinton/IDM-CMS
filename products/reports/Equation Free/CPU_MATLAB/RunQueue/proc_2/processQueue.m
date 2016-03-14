cd('run_1');
try
executeRun;
cd('..');
movefile('run_1', 'done')
catch
 	disp('Error in run 1');
 cd('..');
 end

