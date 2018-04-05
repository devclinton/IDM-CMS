# R script to accompany TB_model.emodl and TB_model_config.cfg and produce trajectories from the output

library( reshape2 )
library( ggplot2 )

setwd( "C:/cms/models" )

output <- t( read.csv( "trajectories.csv", skip=1, header=F ) )
t2_start    <- 350
t2_obser    <- 400


#################################################

colnames( output ) <- output[1,]
output <- output[-1,]

# output <- as.data.frame(sapply(output, as.numeric)) #<- sapply is here
output2 <- apply( output, 2, function(x){ as.numeric(x) } )
output2 <- as.data.frame( output2 )
# colnames( output2 ) <- gsub( ".0.", "", colnames( output2 ) )
colnames( output2 ) <- gsub( "\\}", "", colnames( output2 ) )
colnames( output2 ) <- gsub( "\\{", "|", colnames( output2 ) )
output2.mt <- melt( output2, id.vars="sampletimes" )
states_runNums <- strsplit( as.character( output2.mt$variable ), "\\|" )

output3.mt <- data.frame(
    SampleTime = output2.mt$sampletimes,
    State_RunNum = output2.mt$variable,
    State = unlist( lapply(states_runNums,function(x){x[1]}) ),
    RunNum = factor( as.character( unlist( lapply(states_runNums,function(x){x[2]}) ) ) ),
    Value = output2.mt$value );

p1 <- ggplot( data=output3.mt, aes( x=SampleTime, y=Value, colour=State, group=State_RunNum ) ) +
    geom_vline( xintercept=t2_start, linetype="dashed" ) +
    geom_vline( xintercept=t2_obser, linetype="dashed" ) +
    geom_hline( yintercept=c( 0 ), alpha=0 ) +
    geom_line( alpha=0.3 ) +
    ylab( "Number of individuals" ) +
    theme_bw()
dev.new(); show( p1 )

p1b <- ggplot( data=subset( output3.mt[ which( output3.mt$State %in% c( "infectious_tb" ) ), ], SampleTime>300 ), aes( x=SampleTime, y=Value, colour=State, group=State_RunNum ) ) +
    geom_vline( xintercept=t2_start, linetype="dashed" ) +
    geom_vline( xintercept=t2_obser, linetype="dashed" ) +
    geom_hline( yintercept=c( 0 ), alpha=0 ) +
    geom_line( alpha=0.3 ) +
    ylab( "Number of individuals" ) +
    theme_bw()
dev.new(); show( p1b )


#################################################


runNums <- levels( output3.mt$RunNum )
t2_start_approx <- unique( output3.mt$SampleTime )[ which.min( abs( unique( output3.mt$SampleTime ) - t2_start ) ) ]
t2_obser_approx <- unique( output3.mt$SampleTime )[ which.min( abs( unique( output3.mt$SampleTime ) - t2_obser ) ) ]
state_obser <- "infectious_tb"

diff_list <- list()
for( i in 1:length( runNums ) ) {
    start_val <- subset( output3.mt, SampleTime==t2_start_approx & State==state_obser & RunNum==runNums[i] )["Value"]
    obser_val <- subset( output3.mt, SampleTime==t2_obser_approx & State==state_obser & RunNum==runNums[i] )["Value"]
    diff_val  <- 100 * ( obser_val - start_val ) / start_val
    diff_list <- append( diff_list, diff_val )
}
diff_list <- unlist( diff_list )
diff_list.df <- data.frame( Percent_Change=diff_list )

sample_size  <- sum( subset(output3.mt, SampleTime==0 & RunNum=="1" )$Value )

options( scipen=100 )
phist <- ggplot( data=diff_list.df, aes( x=diff_list.df$Percent_Change ) ) +
    geom_histogram( aes( y =..density.. ), alpha=0.5, fill="red" ) +
    geom_density( size=1, alpha=0.5 ) +
    # geom_vline( xintercept=as.numeric( diff_val_ref ), size=1, linetype="longdash", alpha=0.5 ) +
    xlab( paste( "% change in prevalence, init pop = ", sample_size, sep="" ) ) +
    ylab( "Density" ) +
    theme_bw()
dev.new(); show( phist )

pdf( "tb_model_cms_outputs.pdf", height=5, width=8 )
print( p1 )
print( p1b )
print( phist )
dev.off()

