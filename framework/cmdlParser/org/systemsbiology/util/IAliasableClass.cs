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

	/// <summary> Marker interface defining a class that contains
	/// a "CLASS_ALIAS" field of type String (public).
	/// The value of this field is the &quot;alias&quot;
	/// for the class, a short name used by the class
	/// registry built in the {@link ClassRegistry} class.
	///
	/// </summary>
	/// <author>  Stephen Ramsey
	/// </author>
	public struct IAliasableClass_Fields{
		public readonly static System.String CLASS_ALIAS = "classalias";
	}
	public interface IAliasableClass
	{
		//UPGRADE_NOTE: Members of interface 'IAliasableClass' were extracted into structure 'IAliasableClass_Fields'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1045'"
	}
}