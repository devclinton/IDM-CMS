function Equations = CreateCMDLFile2(param, IC)

n=param.n;


Equations(1,:)={['S1=',num2str(IC(1)),';']};

for j=2:n
    Equations(end+1,:)={['S',num2str(j),'=',num2str(IC(j)),';']};
end

for j=1:n
    Equations(end+1,:)={['I',num2str(j),'=',num2str(IC(j+n)),';']};
end

for j=1:n
    Equations(end+1,:)={['R',num2str(j),'=',num2str(IC(j+2*n)),';']};
end

Equations(end+1,:)={['nu=',num2str(param.nu),';']};
Equations(end+1,:)={['N=',num2str(param.N),';']};
Equations(end+1,:)={['alpha=',num2str(param.alpha),';']};
Equations(end+1,:)={['beta=',num2str(param.beta),';']};
Equations(end+1,:)={['mu=',num2str(param.mu),';']};
Equations(end+1,:)={['gamma=',num2str(param.gamma),';']};



% Toy Model
%Creation
%Create Birth String
BirthString = [];

j=1;

   BirthString =  strcat(BirthString,['nu*S',num2str(j)]);
   BirthString =  strcat(BirthString,['+nu*I',num2str(j)]);
   BirthString =  strcat(BirthString,['+nu*R',num2str(j)]);

for j = 2:n
   BirthString =  strcat(BirthString,['+nu*S',num2str(j)]);
   BirthString =  strcat(BirthString,['+nu*I',num2str(j)]);
   BirthString =  strcat(BirthString,['+nu*R',num2str(j)]);
end

Equations(end+1,:)=  {['Birth',num2str(j),', ->S1,[(',BirthString,')*nu];']};

% Aging
for j=2:n
Equations(end+1,:)=  {['AgingS',num2str(j-1),', S',num2str(j-1),'->S',num2str(j),',alpha;']}; %#ok<*AGROW>
Equations(end+1,:)=  {['AgingI',num2str(j-1),', I',num2str(j-1),'->I',num2str(j),',alpha;']};
Equations(end+1,:)=  {['AgingR',num2str(j-1),', R',num2str(j-1),'->R',num2str(j),',alpha;']};
end

%Death
for j=1:n
   Equations(end+1,:)=  {['DeathS',num2str(j),',S',num2str(j),'-> , mu;']};
   Equations(end+1,:)=  {['DeathI',num2str(j),',I',num2str(j),'-> , mu;']};
   Equations(end+1,:)=  {['DeathR',num2str(j),',R',num2str(j),'-> , mu;']};
end

%Recovery
for j=1:n
   Equations(end+1,:)=  {['Recovery',num2str(j-1),',I',num2str(j),'-> R',num2str(j),', gamma;']};
end

%Infection
for jj=1:n
for j=1:n
   Equations(end+1,:)=  {['Infection',num2str(j),num2str(jj),',S',num2str(j),'+I',num2str(jj),'->I',num2str(j),'+I',num2str(jj),',beta/N;']};
end
end

% open the file with write permission
fid = fopen('exp.cmdl', 'w');
fprintf(fid, '%s \n', Equations{:});
fclose(fid);

% view the contents of the file
%type exp2.cmdl


end

