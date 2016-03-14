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

	/// <summary> This empty class is the common root for all persistent capable classes.
	/// If this class inherits from [tt]java.lang.Object[/tt] then all subclasses are serializable with the standard Java serialization mechanism.
	/// If this class inherits from [tt]com.objy.db.app.ooObj[/tt] then all subclasses are [i]additionally[/i] serializable with the Objectivity ODBMS persistance mechanism.
	/// Thus, by modifying the inheritance of this class the entire tree of subclasses can be switched to Objectivity compatibility (and back) with minimum effort.
	/// </summary>
	[Serializable]
	public abstract class PersistentObject:System.Object, System.ICloneable
	{
		public const long serialVersionUID = 1020L;
		/// <summary> Not yet commented.</summary>
		protected internal PersistentObject()
		{
		}
		/// <summary> Returns a copy of the receiver.
		/// This default implementation does not nothing except making the otherwise [tt]protected[/tt] clone method [tt]public[/tt].
		///
		/// </summary>
		/// <returns> a copy of the receiver.
		/// </returns>
		public virtual System.Object Clone()
		{
			try
			{
				return base.MemberwiseClone();
			}
			catch (System.Exception)
			{
				throw new System.ApplicationException(); //should never happen since we are cloneable
			}
		}
	}
}