/*
* Copyright (C) 2003 by Institute for Systems Biology,
* Seattle, Washington, USA.  All rights reserved.
*
* This source code is distributed under the GNU Lesser
* General Public License, the text of which is available at:
*   http://www.gnu.org/copyleft/lesser.html
*/
using System;
using org.systemsbiology.util;
namespace org.systemsbiology.chem
{

	/// <summary> Represents an object that can build a {@link Model} from an
	/// InputStream.  Typically, the input stream is a file or a
	/// string containing a model description.  The
	/// {@link org.systemsbiology.util.IncludeHandler}
	/// is typically only used for file-based model descriptions.
	/// The application developer typically obtains an instance of
	/// a model builder corresponding to the type of model description
	/// file (SBML, CMDL, etc.), and passes the model description to the
	/// {@link #buildModel(InputStream, IncludeHandler)} method as an InputStream.
	/// The {@link Model} instance that was built, is returned from this method.
	/// All subclasses of this interface
	/// should implement {@link org.systemsbiology.util.IAliasableClass},
	/// and have the public static string field [code]CLASS_ALIAS[/code].
	///
	/// </summary>
	/// <author>  sramsey
	///
	/// </author>
	public interface IModelBuilder
	{
		System.String FileRegex
		{
			get;

		}
		Model buildModel(System.IO.Stream pInputStream, IncludeHandler pIncludeHandler);
		System.String readModel(System.IO.Stream pInputStream);

		/// <summary> Write the model to the specified OutputStream, using the correct
		/// character encoding.
		/// </summary>
		void  writeModel(System.String pModelText, System.IO.Stream pOutputStream);
	}
}