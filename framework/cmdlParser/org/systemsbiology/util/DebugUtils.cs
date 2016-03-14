/*
* Copyright (C) 2003 by Institute for Systems Biology,
* Seattle, Washington, USA.  All rights reserved.
*
* This source code is distributed under the GNU Lesser
* General Public License, the text of which is available at:
*   http://www.gnu.org/copyleft/lesser.html
*/

using System;
using System.Collections.Generic;

namespace org.systemsbiology.util
{

    public class DebugUtils
    {
        public static bool Debug
        {
            get
            {
                return (sDebug);
            }

            set
            {
                sDebug = value;
            }

        }
        private static bool sDebug;

        public static void  printDoubleVector(double[] pVec)
        {
            int numElements = pVec.Length;
            for (int ctr = 0; ctr < numElements; ++ctr)
            {
                System.Console.Out.WriteLine("index: " + ctr + "; value: " + pVec[ctr]);
            }
        }


        public static void describeSortedObjectList(System.Text.StringBuilder pStringBuffer, IList<Object> pObjects, System.Type pClassTypeFilter)
        {
            System.String separatorString = ", ";
            describeSortedObjectList(pStringBuffer, pObjects, pClassTypeFilter, separatorString);
        }


        public static void describeSortedObjectList(System.Text.StringBuilder pStringBuffer, IList<Object> pObjects, System.String pSeparatorString)
        {
            System.Type classTypeFilter = null;
            describeSortedObjectList(pStringBuffer, pObjects, classTypeFilter, pSeparatorString);
        }


        public static void describeSortedObjectList(System.Text.StringBuilder pStringBuffer, IList<Object> pObjects)
        {
            System.String separatorString = ", ";
            System.Type classTypeFilter = null;
            describeSortedObjectList(pStringBuffer, pObjects, classTypeFilter, separatorString);
        }


        public static void describeSortedObjectList(System.Text.StringBuilder pStringBuffer, IList<Object> pObjects, System.Type pClassTypeFilter, System.String pSeparatorString)
        {
/*
            System.Collections.IList objectList = new System.Collections.ArrayList(pObjects.Values);
            SupportClass.CollectionsSupport.Sort(objectList, null);
            System.Collections.IEnumerator iter = objectList.GetEnumerator();
*/
            System.Text.StringBuilder sb = pStringBuffer;
            sb.Append("{\n");
            bool first = true;

            //while (iter.MoveNext())
            foreach (Object obj in pObjects)
            {
                //System.Object obj = iter.Current;

                if (null != pClassTypeFilter)
                {
                    if (!(obj.GetType().IsAssignableFrom(pClassTypeFilter)))
                    {
                        continue;
                    }
                }

                if (!first)
                {
                    sb.Append(pSeparatorString);
                }
                else
                {
                    first = false;
                }

                //UPGRD_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                sb.Append(obj.ToString());
            }

            sb.Append("\n}");
        }

        static DebugUtils()
        {
            {
                sDebug = false;
            }
        }
    }
}