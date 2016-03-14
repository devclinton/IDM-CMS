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
            NewZero = sum(~NonZero);
            p0 = currentMacro.x(end);
            
            p = p0 + (1-p0)/length(Wild)*NewZero;
            macro = macroState(currentMacro.t+dataSet.time, ...
                [MeanVal(:); p]);
            
        end
        
        
        function ICList = lift(obj, macro, NTrajectories)

            meanvals = macro.x(1:end-1);
            ICList = round(repmat(meanvals, 1, NTrajectories));
            ICList(ICList<0) = 0;
            ICList = dataSet(ICList);

        end
        
        
        
        
    end
    
    
end