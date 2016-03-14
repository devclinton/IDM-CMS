using System;
namespace org.systemsbiology.util
{
	/*
	* Copyright (C) 2003 by Institute for Systems Biology,
	* Seattle, Washington, USA.  All rights reserved.
	*
	* This source code is distributed under the GNU Lesser
	* General Public License, the text of which is available at:
	*   http://www.gnu.org/copyleft/lesser.html
	*/

	/// <summary> Represents an error condition in which the caller has requested
	/// a particular data element, but the data element was not found.
	///
	/// </summary>
	/// <author>  Stephen Ramsey
	/// </author>

    [Serializable]
	public class DataNotFoundException:System.Exception
	{
		public DataNotFoundException(System.String pMessage):base(pMessage)
		{
		}

		//UPGRADE_NOTE: Exception 'java.lang.Throwable' was converted to 'System.Exception' which has different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
		public DataNotFoundException(System.String pMessage, System.Exception pCause):base(pMessage, pCause)
		{
		}
	}
}