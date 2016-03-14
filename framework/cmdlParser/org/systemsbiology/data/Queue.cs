using System;
namespace org.systemsbiology.data
{

	/*
	* Copyright (C) 2003 by Institute for Systems Biology,
	* Seattle, Washington, USA.  All rights reserved.
	*
	* This source code is distributed under the GNU Lesser
	* General Public License, the text of which is available at:
	*   http://www.gnu.org/copyleft/lesser.html
	*/

	/// <summary> Queue interface.</summary>


	public abstract class Queue
	{
		public abstract bool add(System.Object pElement);
		public abstract System.Object peekNext();
		public abstract System.Object getNext();
	}
}