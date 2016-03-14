classdef EQFScales
    
    properties
        NSpecies = 10;
    end
    
    methods
        
        function obj = EQFScales()
        end
        
        function macro = MacroFromIC(obj, dataSet)
            dummyMacro = macroState(0.0, zeros(obj.NSpecies+1,1));
            macro = obj.restrict(dummyMacro, dataSet);
        end
        
        function macro = restrict(obj, currentMacro, dataSet)
            
            Wild = sum(dataSet.data(2:5,:),1);
            NonZero = Wild>0;
            MeanVal = mean(dataSet.data(:,NonZero),2);
            StdVal = std(dataSet.data(:,NonZero), 0, 2);
            NewZero = sum(~NonZero);
            p0 = currentMacro.x(end);
            
            p = p0 + (1-p0)/length(Wild)*NewZero;
            macro = macroState(currentMacro.t+dataSet.time, ...
                [MeanVal(:); StdVal(:); p]);
            
        end
        
        
        function ICList = lift(obj, macro, NTrajectories)

            
            meanvals = macro.x(1:obj.NSpecies);
            stdvals = macro.x(obj.NSpecies+1:2*obj.NSpecies);
            
            ICList = zeros(obj.NSpecies, NTrajectories);
            
            for ii = 1:obj.NSpecies
                ICList(ii,:) = meanvals(ii) + stdvals(ii)*randn(1,NTrajectories);
            end
                        
            ICList(ICList<0) = 0;
            ICList = round(ICList);
            ICList = dataSet(ICList);

        end
        
        
        
        
    end
    
    
end