function Equations = CreateConfigFile(param)



Equations(1,1)={['{']};
Equations(end+1,1)={['"solver":  "',param.Solver,'",']};
Equations(end+1,1)={['"runs":  ',num2str(param.NumTrajectories),',']};
Equations(end+1,1)={['"duration":  ',num2str(param.Duration),',']};
Equations(end+1,1)={['"samples":  ',num2str(param.WithinTrajSamples),',']};


if (strcmp(param.Solver,'Bleaping')) || (strcmp(param.Solver,'BLeap')) || (strcmp(param.Solver,'B'))
    Equations(end+1,1)={['"b-leaping": { ']};
    Equations(end+1,1)={['"Tau": ',param.Tau,',']};
    Equations(end+1,1)={['}']};
end

if (strcmp(param.Solver,'tauleaping')) || (strcmp(param.Solver,'TAU')) || (strcmp(param.Solver,'Tau')) ...
        || (strcmp(param.Solver,'TAUFAST')) || (strcmp(param.Solver,'TAULEAPINGFAST'))
    Equations(end+1,1)={['"tau-leaping": { ']};
    Equations(end+1,1)={['"epsilon": ',num2str(param.epsilon),'']};
    
    if isfield(param,'Nc')
        Equations(end,1)=strcat(Equations(end,:),',');
         Equations(end+1,1)={['"Nc": ',num2str(param.Nc),'']};
    end
    if isfield(param,'Multiple')
         Equations(end,1)=strcat(Equations(end,:),',');
        Equations(end+1,1)={['"Multiple": ',num2str(param.Multiple),'']};
    end
    if isfield(param,'SSAruns')
         Equations(end,1)=strcat(Equations(end,:),',');
        Equations(end+1,1)={['"SSAruns": ',num2str(param.SSAruns),'']};
    end
    if isfield(param,'delta')
         Equations(end,1)=strcat(Equations(end,:),',');
        Equations(end+1,1)={['"delta": ',num2str(param.delta),'']};
    end
    Equations(end+1,1)={['}']};
end

Equations(end+1,1)={['}']};

% open the file with write permission
fid = fopen('config.json', 'w');
fprintf(fid, '%s \n', Equations{:});
fclose(fid);

end