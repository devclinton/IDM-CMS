
AgeBins = n;

t=linspace(0,str2num(Co.duration),str2num(Co.samples));


    for jj=1:length(F.ChannelTitles)
        
        G=F.ChannelTitles{jj};
        
        if G(3)=='{'
        Index=str2num(G(2));
        IndexRun=str2num(G(4:end-1));
        else
        Index=str2num(G(2:3));
        IndexRun=str2num(G(5:end-1));
        end
        
        Data=cell2mat(F.ChannelData{jj});
        
        if (G(1)=='S') 
            
            figure(1)
            
            subplot(2,5,Index+jjj*param.n), plot(t,Data,'b'), hold on
            set(gca,'FontSize',[22],'LineWidth',1.2)
            title(['AgeBin   ',num2str(Index)])
            axis([0 str2num(Co.duration) 0 param.N])
            
            if Index==5
            ylabel('Susc')    
            end
            
            elseif (G(1)=='I')
            
            figure(2)
            
            
            subplot(2,5,Index+jjj*param.n), plot(t,Data,'r'), hold on
            set(gca,'FontSize',[22],'LineWidth',1.2)
            title(['AgeBin  ',num2str(Index)])
            axis([0 str2num(Co.duration) 0 param.N])
            
            if Index==5
            ylabel('Infect')    
            end
            
        elseif (G(1)=='R')
            
           figure(3)
           
            subplot(2,5,Index+jjj*param.n), plot(t,Data,'g'), hold on
            set(gca,'FontSize',[22],'LineWidth',1.2)
            title(['AgeBin  ',num2str(Index)])
            axis([0 str2num(Co.duration) 0 param.N])
            
            if Index==5
            ylabel('Recovered')    
            end
            
        end
        
    end
    
    
    