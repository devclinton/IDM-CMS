function Equations = CreateConfigFile(param)



Equations(1,1)={['{']};
Equations(end+1,1)={['"solver":  "',param.solver,'",']};
Equations(end+1,1)={['"runs":  ',param.runs,',']};
Equations(end+1,1)={['"duration":  ',param.duration,',']};
Equations(end+1,1)={['"samples":  ',param.samples,',']};


if (strcmp(param.solver,'Bleaping')) || (strcmp(param.solver,'BLeap')) || (strcmp(param.solver,'B'))
    Equations(end+1,1)={['"b-leaping": { ']};
    Equations(end+1,1)={['"Tau": ',param.Tau,',']};
    Equations(end+1,1)={['}']};
end

if (strcmp(param.solver,'tauleaping')) || (strcmp(param.solver,'TAU')) || (strcmp(param.solver,'Tau'))
    Equations(end+1,1)={['"tau-leaping": { ']};
    Equations(end+1,1)={['"epsilon": ',param.epsilon,',']};
    Equations(end+1,1)={['"Nc": ',param.Nc,',']};
    Equations(end+1,1)={['"Multiple": ',param.Multiple,',']};
    Equations(end+1,1)={['"SSAruns": ',param.SSAruns,',']};
    Equations(end+1,1)={['}']};
end

Equations(end+1,1)={['}']};

% open the file with write permission
fid = fopen('config.json', 'w');
fprintf(fid, '%s \n', Equations{:});
fclose(fid);

end