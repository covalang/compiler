using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Cova.Symbols;

namespace Cova
{
	class SymbolResolutionListener : CovaParserBaseListener
	{
		private ISymbol symbol;
        private readonly Stack<ParserRuleContext> stack = new(64);

        public SymbolResolutionListener(ISymbol rootSymbol) => symbol = rootSymbol;

        public override void EnterEveryRule([NotNull] ParserRuleContext context)
        {
            stack.Push(context)
        }

        public override void ExitEveryRule([NotNull] ParserRuleContext context)
        {
            stack.Pop();
        }
    }
}