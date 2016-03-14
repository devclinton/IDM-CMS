classdef EQFScales
    
    properties
        Energy = 1e-3; 
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
            [U,S,V] = svd(dataSet.data(:,NonZero) - repmat(meanvals, 1, size(dataSet.data(:,NonZero),2)),...
                'econ');
            V = V.';
            S = diag(S);

            En = S/sum(S); % mode energy
            Val = En > obj.Energy;
            

            % Retain only necessary modes
            %U(:,6:end) = 0;
            %S = diag(S);  S(6:end) = 0;
            %V(:,6:end) = 0;
            p0 = currentMacro.x(end);
            p = p0 + (1-p0)/size(dataSet.data,2)*sum(~NonZero);
           
            macro = macroState(currentMacro.t+dataSet.time, ...
                [meanvals(:); U(:); S(:); V(:); p]);
        end
        
        
        function ICList = lift(obj, macro, NTrajectories)

            meanvals = macro.x(1:obj.NSpecies);
            SVDvals = reshape(macro.x(obj.NSpecies+1:end-1), obj.NSpecies, []);
            p = macro.x(end);
            
            U = SVDvals(:,1:obj.NSpecies);
            S = SVDvals(:,obj.NSpecies+1);
            V = SVDvals(:,obj.NSpecies+2:end).';

            randV = zeros(size(V,2), NTrajectories);
            
            for ii = 1:size(randV,1)
               plist = ((1:size(V,1))-0.5)/size(V,1);
               vals = sort(V(:,ii));
               
               randV(ii,:) = interp1(plist, vals.', rand(1,NTrajectories),'linear', 'extrap');
                
            end
            
            ICList = U*diag(S)*randV + repmat(meanvals(:), 1, size(randV,2));
            

            ICList = round(ICList);
            ICList(ICList<0) = 0;
            ICList = dataSet(ICList);

        end
        
        
        
        
    end
    
    
end