using Cova.Symbols;

namespace Cova
{
	class SymbolResolutionListener : CovaParserBaseListener
	{
		private ISymbol symbol;

		public SymbolResolutionListener(ISymbol rootSymbol) => symbol = rootSymbol;
	}
}