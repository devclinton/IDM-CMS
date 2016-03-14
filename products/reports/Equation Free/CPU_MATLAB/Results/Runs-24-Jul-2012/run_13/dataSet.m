classdef dataSet
    
    properties 
        time
        data
        NSpecies
        NTrajectories
    end
    
    methods
        function obj = dataSet(dataArray)
            
           if iscell(dataArray)
               obj.NSpecies = length(dataArray)-1;
               obj.NTrajectories = length(dataArray{1});
               obj.time = dataArray{1}(1);
               obj.data = zeros(obj.NSpecies, obj.NTrajectories);
               
               for ii = 1:obj.NSpecies
                   obj.data(ii,:) = dataArray{ii+1};
               end
           else
               obj.NSpecies = size(dataArray,1);
               obj.NTrajectories = size(dataArray,2);
               obj.time = 0;
               obj.data = dataArray;
           end
           
        end
        
        function NonErad = NonEradicated(obj)
            Wild = sum(obj.data(2:5,:),1)>0;
            NonErad = obj.data(:,Wild);
        end
        
        function Erad = Eradicated(obj)
           Wild = sum(obj.data(2:5,:),1)<1;
           Erad = obj.data(:,Wild);
        end
        
    end
end