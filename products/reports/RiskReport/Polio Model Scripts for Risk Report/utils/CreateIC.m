function IC = CreateIC(pa,param)

A = zeros(pa.numberOfAgeGroups);

for j = 1 : pa.numberOfAgeGroups
   
    A(j,j) = pa.AgingVector(j) + pa.DyingVector(j);
    
    if j ~= pa.numberOfAgeGroups
        
        A(j+1,j) = -pa.AgingVector(j);
        
    end
    
end

B = zeros(pa.numberOfAgeGroups,1);
B(1) = param.nu;
x = inv(A) * B;

    IC = [round(x*param.N*pa.accessGroupPercent(1));  
          zeros(pa.InfectedWildComp*pa.numberOfAgeGroups ...
          + pa.InfectedVaccComp*pa.numberOfAgeGroups ...
          +pa.numberOfAgeGroups,1)];   
      
      temp = IC;
      
      if pa.numNodes > 1
     
          for j = 2 : pa.numNodes
             
              IC = [IC;temp];
              
          end
          
      end

end