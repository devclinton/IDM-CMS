/*
* Copyright (C) 2003 by Institute for Systems Biology,
* Seattle, Washington, USA.  All rights reserved.
*
* This source code is distributed under the GNU Lesser
* General Public License, the text of which is available at:
*   http://www.gnu.org/copyleft/lesser.html
*/
using System;
using org.systemsbiology.math;
using org.systemsbiology.util;
namespace org.systemsbiology.chem
{

	public sealed class ReservedSymbolMapperChemCommandLanguage:ReservedSymbolMapper
	{
		override public System.Collections.ICollection ReservedSymbolNames
		{
			get
			{

				return (new SupportClass.HashSetSupport(sReservedSymbolNames));
			}

		}
		public const System.String SYMBOL_TIME = "time";
		public const System.String SYMBOL_AVOGADRO = "Navo";


		private static SupportClass.HashSetSupport sReservedSymbolNames;


		private static void  getReservedSymbolNamesStatic(SupportClass.HashSetSupport pReservedSymbolNames)
		{
			pReservedSymbolNames.Add(SYMBOL_TIME);
			pReservedSymbolNames.Add(SYMBOL_AVOGADRO);
		}

		public static bool isReservedSymbol(System.String pSymbolName)
		{
			return (sReservedSymbolNames.Contains(pSymbolName));
		}

		public override bool isReservedSymbol(Symbol pSymbol)
		{
			return (sReservedSymbolNames.Contains(pSymbol.Name));
		}

		public override double getReservedSymbolValue(Symbol pSymbol, SymbolEvaluator pSymbolEvaluator)
		{
			System.String symbolName = pSymbol.Name;
			if (symbolName.Equals(SYMBOL_AVOGADRO))
			{
				return (Constants.AVOGADRO_CONSTANT);
			}
			else if (symbolName.Equals(SYMBOL_TIME))
			{
				return (((SymbolEvaluatorChem) pSymbolEvaluator).Time);
			}
			else
			{
				throw new DataNotFoundException("symbol is not a reserved symbol: \"" + symbolName + "\"");
			}
		}
		static ReservedSymbolMapperChemCommandLanguage()
		{
			{

				sReservedSymbolNames = new SupportClass.HashSetSupport();
				getReservedSymbolNamesStatic(sReservedSymbolNames);
			}
		}
	}
}