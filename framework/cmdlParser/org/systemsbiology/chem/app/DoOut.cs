using System;
namespace org.systemsbiology.chem.app
{

	public class DoOut
	{

		public virtual void  dOut(System.String msg, bool lf)
		{
			System.String fMsg = null;

			fMsg = msg + (lf?"\n":"");
			System.Console.Out.WriteLine(fMsg);
		}
	}
}