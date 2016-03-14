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
            
            meanvals = mean(dataSet.data(:,NonZero),2);
            corr = cov(dataSet.data(:,NonZero).');
            [V,D] = eig(corr);

            
            transformed = dataSet.data(:,NonZero) ...
                - repmat(meanvals,1,size(dataSet.data(:,NonZero),2));
            randVals = V'*transformed;
            
            
            
            len = size(dataSet.data(:,NonZero),2);
            p = ((1:len)-0.5)/len;
            
            coeff = [];
            parts = partition(size(randVals,2), obj.dof);
            for ii = 1:size(transformed,1)
                sorted = sort(randVals(ii,:));
                for jj = 1:length(parts)
                    coeff = [coeff; mean(sorted(parts{jj}))];
                end
            end
            
            p0 = currentMacro.x(end);
            p = p0 + (1-p0)/size(dataSet.data,2)*sum(~NonZero);
            
            macro = macroState(currentMacro.t+dataSet.time, ...
                [meanvals(:); corr(:); coeff(:); p]);
            
        end
        
        
        function ICList = lift(obj, macro, NTrajectories)

            meanvals = macro.x(1:obj.NSpecies);
            corrvals = reshape(macro.x(obj.NSpecies+1:obj.NSpecies^2+obj.NSpecies),...
                obj.NSpecies, obj.NSpecies);
            coeffs = reshape(macro.x(obj.NSpecies+obj.NSpecies^2+1:end-1), obj.dof, obj.NSpecies); 
   
            [vecs, vals] = eig(corrvals);
            p = ((1:obj.dof)-0.5)/obj.dof;

            randMat = zeros(obj.NSpecies, NTrajectories);
            for ii = 1:obj.NSpecies
               randMat(ii,:) = interp1(p, coeffs(:,ii), rand(1,NTrajectories), 'linear', 'extrap'); 
            end
            
            ICList = round(vecs*randMat + ...
                repmat(meanvals(:), 1, NTrajectories));
            
            ICList(ICList<0) = 0;
            ICList = dataSet(ICList);

        end
        
        
        
        
    end
    
    
end