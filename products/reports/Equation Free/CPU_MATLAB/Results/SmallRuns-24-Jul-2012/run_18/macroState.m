classdef macroState
    
    properties
        t;
        x;
    end
    
    methods
        function obj = macroState(t,x)
            obj.t = t;
            obj.x = x(:);
        end
        
        function result = plus(a,b)
            result = macroState(a.t+b.t, a.x+b.x);
        end
        
        function result = mtimes(a,b)
            if isa(a,'macroState')
                result = macroState(b*a.t, b*a.x);
            else
                result = macroState(b.t*a, b.x*a);
            end
        end
        
        function result = minus(a,b)
           result = macroState(a.t-b.t, a.x-b.x); 
            
        end
    end
    
    
    
end