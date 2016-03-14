classdef EQFScales
    
    properties
        dof = 7;
        NSpecies = 10;
    end
    
    methods
        
        function obj = EQFScales()
        end
        
        function macro = MacroFromIC(obj, DataSet)
            dummyMacro = macroState(0.0, []);
            macro = obj.restrict(dummyMacro, DataSet);
        end
        
        function macro = restrict(obj, currentMacro, DataSet)

            macro = macroState(currentMacro.t+DataSet.time, ...
                DataSet.data(:));
        end
        
        
        function ICList = lift(obj, macro, NTrajectories)
            FullData = reshape(macro.x, obj.NSpecies, []);
            ICList = dataSet(FullData(:,1:NTrajectories)); 
        end
        
        
        
        
    end
    
    
end