classdef Stepper
    
   properties
      Funs; % Class with lifting and restriction operators
      Simulator;
      k;
      M;
      NRuns;
      microTime = 0;
      totalTime = 0;
   end
    
   methods
       function [obj] = Stepper(Simulator, Funs, k, M, NRuns)
          obj.Funs = Funs; 
          obj.Simulator = Simulator;
          obj.k = k;
          obj.M = M;
          obj.NRuns = NRuns;
       end
       
       
       function macroNew = Step(obj,macro)
           micro = obj.Funs.lift(macro, obj.NRuns);
           macroData = {};
           macroData{1} = macro;

           for ii = 1:obj.k
               t0 = tic;
               micro = obj.Simulator.step(micro);
               obj.microTime = obj.microTime + toc(t0);
               macroData{ii+1} = obj.Funs.restrict(macroData{ii},micro);
           end

           dt = macroData{end}.t-macroData{end-1}.t;
           dData = (1/dt)*(macroData{end}-macroData{end-1});
           macroNew = macroData{end} + obj.M*dt*dData;
       end
   end
   
end