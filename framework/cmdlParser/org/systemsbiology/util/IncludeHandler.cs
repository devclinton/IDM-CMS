/*
* Copyright (C) 2003 by Institute for Systems Biology,
* Seattle, Washington, USA.  All rights reserved.
*
* This source code is distributed under the GNU Lesser
* General Public License, the text of which is available at:
*   http://www.gnu.org/copyleft/lesser.html
*/

using System;

//UPGRADE_TODO: The package 'java.nio.charset' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//J using java.nio.charset;

namespace org.systemsbiology.util
{

	/// <summary> Utility class for handling nested file
	/// inclusion, in a scripting environment.
	///
	/// </summary>
	/// <author>  Stephen Ramsey
	/// </author>
	public class IncludeHandler
	{
		virtual public System.IO.FileInfo Directory
		{
			get
			{
				return (mDirectory);
			}

			set
			{
				if (null != value)
				{
					if (!System.IO.Directory.Exists(value.FullName))
					{
						throw new System.ArgumentException("specified pathname is not a directory: " + value.FullName);
					}
				}
				mDirectory = value;
			}

		}

		private SupportClass.HashSetSupport IncludedFiles
		{
			get
			{
				return (mIncludedFiles);
			}

			set
			{
				mIncludedFiles = value;
			}

		}
		virtual public bool WithinIncludeFile
		{
			get
			{
				return (IncludedFiles.Count > 1);
			}

		}

		private SupportClass.HashSetSupport mIncludedFiles;
		private System.IO.FileInfo mDirectory;

		private bool alreadyParsedFile(System.String pFileName)
		{
			return (IncludedFiles.Contains(pFileName));
		}

		private void  storeParsedFile(System.String pFileName)
		{
			IncludedFiles.Add(pFileName);
		}

		public IncludeHandler()
		{

			IncludedFiles = new SupportClass.HashSetSupport();
			Directory = null;
		}

		public virtual System.String getIncludeFileAbsolutePath(System.String pIncludeFileName)
		{
			System.IO.FileInfo includeFile = new System.IO.FileInfo(pIncludeFileName);
			System.String includeFileAbsolutePath = null;
			//UPGRADE_ISSUE: Method 'java.io.File.isAbsolute' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaioFileisAbsolute'"
			//J if (!includeFile.isAbsolute())
            if (!IsAbsolutePath(pIncludeFileName))
			{
				System.IO.FileInfo dirFile = Directory;
				if (null != dirFile)
				{
					includeFileAbsolutePath = dirFile.FullName + System.IO.Path.DirectorySeparatorChar.ToString() + pIncludeFileName;
				}
				else
				{
					includeFileAbsolutePath = (new System.IO.FileInfo(pIncludeFileName)).FullName;
				}
			}
			else
			{
				includeFileAbsolutePath = includeFile.FullName;
			}
			return (includeFileAbsolutePath);
		}

        private bool IsAbsolutePath(string filename)
        {
            bool isAbsolute = false;

            // Need at least three characters for "[A-Z]:\"
            if (filename.Length > 2)
            {
                // Starts with letter?
                if ((filename[0] >= 'a' && filename[0] <= 'z') ||
                    (filename[0] >= 'A' && filename[0] <= 'Z'))
                {
                    // Followed by ":\"?
                    if (filename[1] == ':' && filename[2] == '\\')
                        isAbsolute = true;
                }
            }

            return isAbsolute;
        }

		//J public virtual System.IO.StreamReader openReaderForIncludeFile(System.String pIncludedFileName, Charset pCharset)
		public virtual System.IO.StreamReader openReaderForIncludeFile(System.String pIncludedFileName)
		{
			System.String includeFileAbsolutePath = getIncludeFileAbsolutePath(pIncludedFileName);
			System.IO.StreamReader bufferedReader = null;
			if (!alreadyParsedFile(includeFileAbsolutePath))
			{
				storeParsedFile(includeFileAbsolutePath);
				System.IO.FileInfo includeFile = new System.IO.FileInfo(includeFileAbsolutePath);
				//UPGRADE_TODO: Constructor 'java.io.FileInputStream.FileInputStream' was converted to 'System.IO.FileStream.FileStream' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileInputStreamFileInputStream_javaioFile'"
				System.IO.FileStream fileInputStream = new System.IO.FileStream(includeFile.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
				System.IO.StreamReader inputStreamReader = null;
/*J
				if (null != pCharset)
				{
					inputStreamReader = new InputStreamReader(fileInputStream, pCharset);
				}
				else
*/
				{
					inputStreamReader = new System.IO.StreamReader(fileInputStream, System.Text.Encoding.Default);
				}
				//UPGRADE_TODO: The differences in the expected value  of parameters for constructor 'java.io.BufferedReader.BufferedReader'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
				bufferedReader = new System.IO.StreamReader(inputStreamReader.BaseStream, inputStreamReader.CurrentEncoding);
			}
			else
			{
				// do nothing
			}
			return (bufferedReader);
		}
	}
}