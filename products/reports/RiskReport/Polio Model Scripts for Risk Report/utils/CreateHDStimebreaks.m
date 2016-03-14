function [tspanvec, SIANodeMatrix] = CreateHDStimebreaks(pa,param)

tspanvec = [];
SIANodeMatrix = [];

    VectorOfSIAtimesAndWhichNode = [];
    
    for j = 1 : pa.numNodes
        
       if strcmp(pa.TypeOfSIACampaign,'Periodic')
        
          
           
           if pa.PeriodicCampaignSameForEachNode == 0
               
               eval(['NoSIAs = isempty(pa.PeriodicCampaignTiming.Node',num2str(j),');']);
               
               if NoSIAs == 1
                   eval(['VectorOfSIAtimesAndWhichNode = [];'])
               else
                    if j == 1         
                        eval(['VectorOfSIAtimesAndWhichNode = [pa.PeriodicCampaignTiming.Node',num2str(j),...
                        '(1,:);',num2str(j),'*ones(1,length(pa.PeriodicCampaignTiming.Node',num2str(j),'(1,:)))];']);
                    else
                        eval(['VectorOfSIAtimesAndWhichNode = [VectorOfSIAtimesAndWhichNode(1,:) pa.PeriodicCampaignTiming.Node',num2str(j),...
                        '(1,:);VectorOfSIAtimesAndWhichNode(2,:) ',num2str(j),'*ones(1,length(pa.PeriodicCampaignTiming.Node',num2str(j),'(1,:)))];']);
                    end
               end
           else
               
               eval(['NoSIAs = isempty(pa.PeriodicCampaignTiming.Node',num2str(1),');']);
               
               if NoSIAs == 1
                   eval(['VectorOfSIAtimesAndWhichNode = [];'])
               else
                    if j == 1         
                        eval(['VectorOfSIAtimesAndWhichNode = [pa.PeriodicCampaignTiming.Node',num2str(1),...
                        '(1,:);',num2str(j),'*ones(1,length(pa.PeriodicCampaignTiming.Node',num2str(1),'(1,:)))];']);
                    else
                        eval(['VectorOfSIAtimesAndWhichNode = [VectorOfSIAtimesAndWhichNode(1,:) pa.PeriodicCampaignTiming.Node',num2str(1),...
                        '(1,:);VectorOfSIAtimesAndWhichNode(2,:) ',num2str(j),'*ones(1,length(pa.PeriodicCampaignTiming.Node',num2str(1),'(1,:)))];']);
                    end
               end
           end
           
       else 
           
           eval(['NoSIAs = isempty(pa.PeriodicCampaignTiming.Node',num2str(j),');']);
           
           if pa.NonPeriodicCampaignSameForEachNode == 0
                if NoSIAs == 1
                   eval(['VectorOfSIAtimesAndWhichNode = [];'])
                else
                    if j == 1         
                         eval(['VectorOfSIAtimesAndWhichNode = [pa.NonPeriodicCampaignTiming.Node',num2str(j),...
                         '(1,:);',num2str(j),'*ones(1,length(pa.NonPeriodicCampaignTiming.Node',num2str(j),'(1,:)))];']);
                    else
                         eval(['VectorOfSIAtimesAndWhichNode = [VectorOfSIAtimesAndWhichNode(1,:) pa.NonPeriodicCampaignTiming.Node',num2str(j),...
                         '(1,:);VectorOfSIAtimesAndWhichNode(2,:) ',num2str(j),'*ones(1,length(pa.NonPeriodicCampaignTiming.Node',num2str(j),'(1,:)))];']);
                    end
                end
           
           else         
                if NoSIAs == 1
                   eval(['VectorOfSIAtimesAndWhichNode = [];'])
                else
                    if j == 1         
                          eval(['VectorOfSIAtimesAndWhichNode = [pa.NonPeriodicCampaignTiming.Node',num2str(1),...
                         '(1,:);',num2str(j),'*ones(1,length(pa.NonPeriodicCampaignTiming.Node',num2str(1),'(1,:)))];']);
                    else
                            eval(['VectorOfSIAtimesAndWhichNode = [VectorOfSIAtimesAndWhichNode(1,:) pa.NonPeriodicCampaignTiming.Node',num2str(1),...
                            '(1,:);VectorOfSIAtimesAndWhichNode(2,:) ',num2str(j),'*ones(1,length(pa.NonPeriodicCampaignTiming.Node',num2str(1),'(1,:)))];']);
                    end
                end
           end
           
           
       end
    end
    
    if ~isempty(VectorOfSIAtimesAndWhichNode)
    sortedVectorOfSIAtimes = sortrows(VectorOfSIAtimesAndWhichNode',1)';
    
    SIANodeMatrix = [];
    Durvec = [];
    DurvecNode = [];
    
    DurvecNodeHold = [];
    
    for j = 1: length(sortedVectorOfSIAtimes)
       
        nextTime = sortedVectorOfSIAtimes(1,j);
        WhichNode = sortedVectorOfSIAtimes(2,j);
        
        if j == 1
                       
            SIANodeMatrix = [SIANodeMatrix;zeros(1,pa.numNodes)]; 
                        
            tspanvec(j,:) = [0 nextTime];
            Durvec = [Durvec nextTime+pa.SIADuration];
            DurvecNode = [DurvecNode WhichNode];
            
            SIANodeMatrix = [SIANodeMatrix;SIANodeMatrix(end,:)];
            SIANodeMatrix(end,DurvecNode(1)) = 1;
            
        else
            
            if nextTime == tspanvec(end,2)
                
                SIANodeMatrix(end,WhichNode) = 1;
            
                Durvec = [Durvec nextTime+pa.SIADuration];
                DurvecNode = [DurvecNode WhichNode];
                
            elseif nextTime < Durvec(1)
                
                tspanvec = [tspanvec;tspanvec(end,2) nextTime];
                SIANodeMatrix = [SIANodeMatrix;SIANodeMatrix(end,:)]; 
                SIANodeMatrix(end,DurvecNode(1)) = 1;
                
                Durvec = [Durvec nextTime+pa.SIADuration];
                DurvecNode = [DurvecNode WhichNode];
                
            elseif nextTime >= Durvec(1)
               
                SameDelay = [];
                
                while nextTime >= Durvec(1)
                
                    if ~isempty(SameDelay)
                        
                        if Durvec(1) == SameDelay
                        
                           SIANodeMatrix(end,DurvecNode(1)) = 0;    
                           Durvec(1) = [];
                           DurvecNode(1) = [];
                           
                            if isempty(Durvec)
                                Durvec = 10000;
                            end
                            
                        else   
                            
                            tspanvec = [tspanvec;tspanvec(end,2) Durvec(1)];
                            SIANodeMatrix = [SIANodeMatrix; SIANodeMatrix(end,:)];
                            SIANodeMatrix(end,DurvecNode(1)) = 0;    

                            SameDelay = Durvec(1);

                            Durvec(1) = [];
                            DurvecNode(1) = [];

                            if isempty(Durvec)
                                Durvec = 10000;
                            end
                        
                        end
                        
                    else
                        
                        tspanvec = [tspanvec;tspanvec(end,2) Durvec(1)];
                        SIANodeMatrix = [SIANodeMatrix; SIANodeMatrix(end,:)];
                        SIANodeMatrix(end,DurvecNode(1)) = 0;    

                        SameDelay = Durvec(1);
                        
                        Durvec(1) = [];
                        DurvecNode(1) = [];

                        if isempty(Durvec)
                            Durvec = 10000;
                        end
                        
                        
                    end
                    

                
                end
                
                Durvec = [];
                
                tspanvec = [tspanvec;tspanvec(end,2) nextTime];
                SIANodeMatrix = [SIANodeMatrix;SIANodeMatrix(end,:)];     
                SIANodeMatrix(end,WhichNode) = 1;
                
                Durvec = [Durvec nextTime+pa.SIADuration];
                DurvecNode = [DurvecNode WhichNode];
                         
            end          
        end                
    end
    
   % This is for any SIA that is still on
         
        SameDelay = [];
        while ~isempty(Durvec)
                 if ~isempty(SameDelay)
                        
                        if Durvec(1) == SameDelay
                        
                           SIANodeMatrix(end,DurvecNode(1)) = 0;    
                           Durvec(1) = [];
                           DurvecNode(1) = [];
                           
                            if isempty(Durvec)
                                break;
                            end
                            
                        else   
                            
                            tspanvec = [tspanvec;tspanvec(end,2) Durvec(1)];
                            SIANodeMatrix = [SIANodeMatrix; SIANodeMatrix(end,:)];
                            SIANodeMatrix(end,DurvecNode(1)) = 0;    

                            SameDelay = Durvec(1);

                            Durvec(1) = [];
                            DurvecNode(1) = [];

                            if isempty(Durvec)
                               break;
                            end
                        
                        end
                        
                    else
                        
                        tspanvec = [tspanvec;tspanvec(end,2) Durvec(1)];
                        SIANodeMatrix = [SIANodeMatrix; SIANodeMatrix(end,:)];
                        SIANodeMatrix(end,DurvecNode(1)) = 0;    

                        SameDelay = Durvec(1);
                        
                        Durvec(1) = [];
                        DurvecNode(1) = [];

                        if isempty(Durvec)
                            break;
                        end
                        
                        
                 end       
        end
        
        
        %%%
   
%          while ~isempty(Durvec)
% 
%                 tspanvec = [tspanvec;tspanvec(end,2) Durvec(1)];
%                 SIANodeMatrix = [SIANodeMatrix; SIANodeMatrix(end,:)]; 
%                 SIANodeMatrix(end,DurvecNode(1)) = 0; 
%                 
%                 Durvec(1) = [];
%                 DurvecNode(1) = [];
% 
%                 if isempty(Durvec)
%                     break;
%                 end
% 
%          end
       
    %This is for the last element in tspan.
        if strcmp(pa.TypeOfSIACampaign,'Periodic')
             tspanvec = [tspanvec;tspanvec(end,2) 1];
        else
            tspanvec = [tspanvec;tspanvec(end,2) pa.Duration];
        end
    
    else
        
        
        if strcmp(pa.TypeOfSIACampaign,'Periodic')
            tspanvec = [0 1];
            SIANodeMatrix = zeros(1,pa.numNodes);
        else
            tspanvec = [0 pa.Duration];
        end
        
        
    end
               

if strcmp(pa.TypeOfSIACampaign,'Periodic')
                
    temp1 = tspanvec;
    temp2 = SIANodeMatrix;
   
    
       for j = 2:ceil(pa.Duration)
          
           tspanvec = [tspanvec;temp1+tspanvec(end,2)];
           SIANodeMatrix = [SIANodeMatrix;temp2];
           
       end
                                
end

end