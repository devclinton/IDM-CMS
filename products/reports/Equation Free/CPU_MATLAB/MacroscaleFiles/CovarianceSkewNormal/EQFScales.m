classdef EQFScales
    
    properties
        dof = 8;
        NSpecies = 10;
    end
    
    methods
        
        function obj = EQFScales()
        end
        
        function macro = MacroFromIC(obj, dataSet)
            dummyMacro = macroState(0.0, zeros(obj.NSpecies^2+2*obj.NSpecies+1));
            macro = obj.restrict(dummyMacro, dataSet);
        end
        
        function macro = restrict(obj, currentMacro, dataSet)
            
            Wild = sum(dataSet.data(2:5,:),1);
            NonZero = Wild>0;
            
            meanvals = mean(dataSet.data(:,NonZero),2);
            corr = cov(dataSet.data(:,NonZero).');
            [V,D] = eig(corr);
            D = diag(D);
            D(D<0) = 0.0;
            randVals = V'*(dataSet.data(:, NonZero) - repmat(meanvals, 1, size(dataSet.data(:,NonZero),2)));
            
            alphavalsIn = currentMacro.x(obj.NSpecies^2+obj.NSpecies+1: obj.NSpecies^2+2*obj.NSpecies);

            alphaval = [];
            for ii = 1:size(randVals,1)
                 alphaval(ii) = mle_skew(randVals(ii,:), 0.0 , sqrt(D(ii)), alphavalsIn(ii));
            end
            
            p0 = currentMacro.x(end);
            p = p0 + (1-p0)/size(dataSet.data,2)*sum(~NonZero);
            
            macro = macroState(currentMacro.t+dataSet.time, ...
                [meanvals(:); corr(:); alphaval(:); p]);
            
        end
        
        
        function ICList = lift(obj, macro, NTrajectories)

            meanvals = macro.x(1:obj.NSpecies);
            corrvals = reshape(macro.x(obj.NSpecies+1:obj.NSpecies^2+obj.NSpecies),...
                obj.NSpecies, obj.NSpecies);
            alphavals = macro.x(obj.NSpecies^2+obj.NSpecies+1: obj.NSpecies^2+2*obj.NSpecies);
   
            [vecs, vals] = eig(corrvals);
            vals = diag(vals);
            vals(vals<0) = 0;
            
            randMat = zeros(obj.NSpecies, NTrajectories);
            for ii = 1:obj.NSpecies
                
                alpha = alphavals(ii);
                delta = alpha/sqrt(1+alpha^2);
                omega = sqrt(vals(ii)/(1-2*delta^2/pi));
                xi = -omega*delta*sqrt(2/pi);
                                
                randMat(ii,:) = randskew(NTrajectories, xi, omega, alpha);
            end
            mean(randMat,2)
            ICList = round(vecs*randMat + ...
                repmat(meanvals(:), 1, NTrajectories));
            
            ICList(ICList<0) = 0;
            ICList = dataSet(ICList);

        end
        
        
        
        
    end
    
    
end