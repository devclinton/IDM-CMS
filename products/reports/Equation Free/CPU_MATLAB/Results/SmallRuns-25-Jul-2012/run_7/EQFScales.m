classdef EQFScales
    
    properties
        dof = 10;
        NSpecies = 10;
    end
    
    methods
        
        function obj = EQFScales()
        end
        
        function macro = MacroFromIC(obj, dataSet)
            dummyMacro = macroState(0.0, zeros(1));
            macro = obj.restrict(dummyMacro, dataSet);
        end
        
        function macro = restrict(obj, currentMacro, dataSet)
            
            Wild = sum(dataSet.data(2:5,:),1);
            NonZero = Wild>0;
            

            
            
            len = size(dataSet.data(:,NonZero),2);
            p = ((1:len)-0.5)/len;
            
            coeff = [];
            parts = partition(size(dataSet.data,2), obj.dof);
            for ii = 1:size(dataSet.data,1)
                sorted = sort(dataSet.data(ii,:));
                for jj = 1:length(parts)
                    coeff = [coeff; mean(sorted(parts{jj}))];
                end
            end
            
            p0 = currentMacro.x(end);
            p = p0 + (1-p0)/size(dataSet.data,2)*sum(~NonZero);
            
            macro = macroState(currentMacro.t+dataSet.time, ...
                [coeff(:); p]);
            
        end
        
        
        function ICList = lift(obj, macro, NTrajectories)

            coeffs = reshape(macro.x(1:end-1), obj.dof, obj.NSpecies); 
   
            p = ((1:obj.dof)-0.5)/obj.dof;

            randMat = zeros(obj.NSpecies, NTrajectories);
            for ii = 1:obj.NSpecies
               randMat(ii,:) = interp1(p, coeffs(:,ii), rand(1,NTrajectories), 'linear', 'extrap'); 
            end
            
            ICList = round(randMat);
            
            ICList(ICList<0) = 0;
            ICList = dataSet(ICList);

        end
        
        
        
        
    end
    
    
end