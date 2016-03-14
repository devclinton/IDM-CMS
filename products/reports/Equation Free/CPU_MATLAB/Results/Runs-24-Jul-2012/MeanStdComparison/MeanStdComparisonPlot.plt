cd 'C:\tmp\eqfTestSuiteNetwork\Results\Runs-24-Jul-2012\MeanStdComparison'   

#set terminal wxt size 600,250
set term pdf enhanced size 6in, 2in
#set term png truecolor enhanced size 1200,400 dashlength 0.5
set output "MeanStdComparison.pdf" 
set termoption dashed

set style line 1 lt 1 lw 3 pt 3 linecolor rgb "red"
set style line 2 lt 3 lw 3 pt 3 linecolor rgb "blue"
set style line 3 lt 1 lw 1 pt 1 linecolor rgb "red"
set style line 4 lt 3 lw 1 pt 1 linecolor rgb "blue"

set multiplot layout 1,3
set style fill transparent solid 0.5

# Plot 1
set title "Endemic"
unset key 
set xlabel "Time [years]"
set ylabel "Wild Type Infected"
unset key
set ytic 250,50
set xrange [0:10]

plot "SSAEndemic.dat" u 1:(($5-$15)):(($5+$15)) w filledcurves ls 1, "SSAEndemic.dat" u 1:($5) w l ls 1, \
	"MeanStdEndemic.dat" u 1:(($5-$15)):(($5+$15)) w filledcurves ls 2, "MeanStdEndemic.dat" u 1:($5) w l ls 2

# Plot 2 
set title "Vaccinated"
unset key 
set xlabel "Time [years]"
set ylabel "Wild Type Infected"
unset key
set xrange [0:10]
set ytic 50,50

plot "SSAVaccinate.dat" u 1:(($5-$15)):(($5+$15)) w filledcurves, "SSAVaccinate.dat" u 1:($5) w l ls 1, \
	"MeanStdVaccinate.dat" u 1:(($5-$15)):(($5+$15)) w filledcurves, "MeanStdVaccinate.dat" u 1:($5) w l ls 2

# Plot 3 
set title "Eradication"
unset key 
set xlabel "Time [years]"
set xrange [0:10]
set ylabel "P(Eradication)"
set yrange [0:1]
set ytic 0,0.2
plot "SSAVaccinate.dat" u 1:($22/10000) w l ls 1, "MeanStdVaccinate.dat" u 1:22 w l ls 2


unset multiplot
exit
