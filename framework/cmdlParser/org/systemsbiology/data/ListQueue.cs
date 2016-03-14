/*
* Copyright (C) 2003 by Institute for Systems Biology,
* Seattle, Washington, USA.  All rights reserved.
*
* This source code is distributed under the GNU Lesser
* General Public License, the text of which is available at:
*   http://www.gnu.org/copyleft/lesser.html
*/

/// <summary> Implementation of a thread-safe FIFO queue using a linked list
/// data structure.
/// </summary>
using System;
namespace org.systemsbiology.data
{

	public sealed class ListQueue:Queue
	{
		private System.Collections.IList mQueue;
		public ListQueue()
		{

			mQueue = new System.Collections.ArrayList();
		}

		//UPGRADE_NOTE: Synchronized keyword was removed from method 'add'. Lock expression was added. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1027'"
		public override bool add(System.Object pElement)
		{
			lock (this)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.util.List.add' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
				return (mQueue.Add(pElement) >= 0);
			}
		}

		//UPGRADE_NOTE: Synchronized keyword was removed from method 'peekNext'. Lock expression was added. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1027'"
		public override System.Object peekNext()
		{
			lock (this)
			{
				System.Object element = null;
				if (mQueue.Count > 0)
				{
					element = mQueue[0];
				}
				return (element);
			}
		}

		//UPGRADE_NOTE: Synchronized keyword was removed from method 'getNext'. Lock expression was added. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1027'"
		public override System.Object getNext()
		{
			lock (this)
			{
				System.Object element = null;
				if (mQueue.Count > 0)
				{
					System.Object tempObject;
					tempObject = mQueue[0];
					mQueue.RemoveAt(0);
					element = tempObject;
				}
				return (element);
			}
		}
	}
}