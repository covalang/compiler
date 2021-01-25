using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;

namespace Cova
{
	public class SemanticAnalyzer : IParseTreeListener
	{
		readonly RootScope rootScope = new RootScope();
		//Func<ParserRuleContext, Boolean>? NamedScopePredicate { get; set; } = x => x.GetType().Name == "NamespaceDefinitionContext";
		//Func<ParserRuleContext, Boolean> AnonymousScopePredicate { get; set; }// = x => x.GetType().Name == "NamespaceDefinitionContext";
		//Func<ParserRuleContext, Boolean> AliasPredicate { get; set; }

		public void EnterEveryRule(ParserRuleContext ctx)
		{
			//if (NamedScopePredicate?.Invoke(ctx))
			//	rootScope.Children.Add(new NamedScope(rootScope) { });
			var what = ctx switch
			{
				IFileScope and IFileSymbol => "wow",
				_ => throw new NotImplementedException()
			};
		}

		public void ExitEveryRule(ParserRuleContext ctx) { }

		public void VisitErrorNode(IErrorNode node) { }

		public void VisitTerminal(ITerminalNode node) { }
	}
}
