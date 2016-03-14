/*
Copyright © 1999 CERN - European Organization for Nuclear Research.
Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose
is hereby granted without fee, provided that the above copyright notice appear in all copies and
that both that copyright notice and this permission notice appear in supporting documentation.
CERN makes no representations about the suitability of this software for any purpose.
It is provided "as is" without expressed or implied warranty.*/
using System;
namespace cern.colt
{

    /// <summary> A handy stopwatch for benchmarking.
    /// Like a real stop watch used on ancient running tracks you can start the watch, stop it,
    /// start it again, stop it again, display the elapsed time and reset the watch.
    /// </summary>
    [Serializable]
    public class Timer:PersistentObject
    {
        private long baseTime;
        private long elapsedTime_Renamed_Field;

        private const long UNIT = 1000;

        /// <summary> Constructs a new timer, initially not started. Use start() to start the timer.</summary>
        public Timer()
        {
            this.reset();
        }

        /// <summary> Prints the elapsed time on System.out</summary>
        /// <returns> [tt]this[/tt] (for convenience only).
        /// </returns>
        public virtual Timer display()
        {
            //UPGRADE_TODO: Method 'java.io.PrintStream.println' was converted to 'System.Console.Out.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintStreamprintln_javalangObject'"
            System.Console.Out.WriteLine(this);
            return this;
        }

        /// <summary> Same as [tt]seconds()[/tt].</summary>
        public virtual float elapsedTime()
        {
            return seconds();
        }

        /// <summary> Returns the elapsed time in milli seconds; does not stop the timer, if started.</summary>
        public virtual long millis()
        {
            long elapsed = elapsedTime_Renamed_Field;
            if (baseTime != 0)
            {
                // we are started
                elapsed += (System.DateTime.Now.Ticks - 621355968000000000) / 10000 - baseTime;
            }
            return elapsed;
        }

        /// <summary> [tt]T = this - other[/tt]; Constructs and returns a new timer which is the difference of the receiver and the other timer.
        /// The new timer is not started.
        /// </summary>
        /// <param name="other">the timer to subtract.
        /// </param>
        /// <returns> a new timer.
        /// </returns>
        public virtual Timer minus(Timer other)
        {
            Timer copy = new Timer();
            copy.elapsedTime_Renamed_Field = millis() - other.millis();
            return copy;
        }

        /// <summary> Returns the elapsed time in minutes; does not stop the timer, if started.</summary>
        public virtual float minutes()
        {
            return seconds() / 60;
        }

        /// <summary> [tt]T = this + other[/tt]; Constructs and returns a new timer which is the sum of the receiver and the other timer.
        /// The new timer is not started.
        /// </summary>
        /// <param name="other">the timer to add.
        /// </param>
        /// <returns> a new timer.
        /// </returns>
        public virtual Timer plus(Timer other)
        {
            Timer copy = new Timer();
            copy.elapsedTime_Renamed_Field = millis() + other.millis();
            return copy;
        }

        /// <summary> Resets the timer.</summary>
        /// <returns> [tt]this[/tt] (for convenience only).
        /// </returns>
        public virtual Timer reset()
        {
            elapsedTime_Renamed_Field = 0;
            baseTime = 0;
            return this;
        }

        /// <summary> Returns the elapsed time in seconds; does not stop the timer, if started.</summary>
        public virtual float seconds()
        {
            return ((float) millis()) / UNIT;
        }

        /// <summary> Starts the timer.</summary>
        /// <returns> [tt]this[/tt] (for convenience only).
        /// </returns>
        public virtual Timer start()
        {
            baseTime = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
            return this;
        }

        /// <summary> Stops the timer. You can start it again later, if necessary.</summary>
        /// <returns> [tt]this[/tt] (for convenience only).
        /// </returns>
        public virtual Timer stop()
        {
            if (baseTime != 0)
            {
                elapsedTime_Renamed_Field = elapsedTime_Renamed_Field + ((System.DateTime.Now.Ticks - 621355968000000000) / 10000 - baseTime);
            }
            baseTime = 0;
            return this;
        }

        /// <summary> Shows how to use a timer in convenient ways.</summary>
        public static void  test(int size)
        {
            //benchmark this piece
            Timer t = new Timer().start();
            int j = 0;
            for (int i = 0; i < size; i++)
            {
                j++;
            }
            t.stop();
            t.display();
            System.Console.Out.WriteLine("I finished the test using " + t);



            //do something we do not want to benchmark
            j = 0;
            for (int i = 0; i < size; i++)
            {
                j++;
            }



            //benchmark another piece and add to last benchmark
            t.start();
            j = 0;
            for (int i = 0; i < size; i++)
            {
                j++;
            }
            t.stop().display();



            //benchmark yet another piece independently
            t.reset(); //set timer to zero
            t.start();
            j = 0;
            for (int i = 0; i < size; i++)
            {
                j++;
            }
            t.stop().display();
        }

        /// <summary> Returns a String representation of the receiver.</summary>
        public override System.String ToString()
        {
            return "Time=" + this.elapsedTime().ToString() + " secs";
        }

        //UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
        override public System.Object Clone()
        {
            throw new NotImplementedException();
        }
    }
}