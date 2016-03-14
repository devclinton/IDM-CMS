using System;
using org.systemsbiology.util;
namespace org.systemsbiology.math
{

	public abstract class ReservedSymbolMapper
	{
		public abstract System.Collections.ICollection ReservedSymbolNames{get;}
		public abstract bool isReservedSymbol(Symbol pSymbol);
		public abstract double getReservedSymbolValue(Symbol pSymbol, SymbolEvaluator pSymbolEvaluator);
	}
}