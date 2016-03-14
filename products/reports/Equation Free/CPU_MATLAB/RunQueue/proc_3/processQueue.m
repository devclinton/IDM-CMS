cd('run_2');
try
executeRun;
cd('..');
movefile('run_2', 'done')
catch
 	disp('Error in run 2');
 cd('..');
 end

