using Antlr4.Runtime.Misc;
using Cova.Symbols;
using System;

using static CovaParser;

namespace Cova
{
	class SymbolRegistrationVisitor : CovaParserBaseVisitor<Byte>
	{
		private ISymbol symbol;
		private readonly SymbolResolver symbolResolver = new();
		public SymbolRegistrationVisitor(IModule rootSymbol) => symbol = rootSymbol;

		public override Byte VisitNamespaceDefinition(NamespaceDefinitionContext context)
		{
			var hold = symbol;
			foreach (var identifier in context.qualifiedIdentifier().identifier())
			{
				var @namespace = InterfaceImplementor.CreateAndInitialize<INamespace>();
				@namespace.DefinitionSource = context.ToTextSourceSpan();
				@namespace.Parent = symbol;
				(symbol as IHasNamespaces)?.Namespaces.Add(@namespace);
				symbol.Children.Add(@namespace);
				@namespace.Name = identifier.GetText();
				symbolResolver.TryRegister(@namespace, out _);
				symbol = @namespace;
			}
			
			base.VisitNamespaceDefinition(context);

			symbol = hold;
			return 0;
		}
	}
}
