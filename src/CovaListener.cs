using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Cova.Symbols;

namespace Cova
{
	class CovaListener : CovaParserBaseListener
	{
		private const Char QualifierDelimiter = '.';
		
		readonly IModule module;
		IScope scope;
		Type? type;
		IFunction? function;
		IDefinition? definition;
		
		public CovaListener(IModule module, IScope scope)
		{
			this.module = module;
			this.scope = scope;
		}

		private readonly List<ParserRuleContext> qualifiers = new();
		private String CurrentQualifier => String.Join(QualifierDelimiter, CurrentQualifiers.Select(x => x.GetText()));

		private IEnumerable<CovaParser.IdentifierContext> CurrentQualifiers => qualifiers
			.SelectMany(
				x => x switch
				{
					CovaParser.QualifiedIdentifierContext qic => qic.identifier(),
					CovaParser.IdentifierContext ic => new[] { ic },
					_ => throw new InvalidOperationException()
				}
			);

		public override void EnterNamespaceDefinition(CovaParser.NamespaceDefinitionContext context)
		{
			qualifiers.AddRange(context.qualifiedIdentifier().identifier());
			var @namespace = InterfaceImplementor.Create<INamespace>();
			@namespace.Name = context.qualifiedIdentifier
			scope = @namespace;
		}

		public override void ExitNamespaceDefinition(CovaParser.NamespaceDefinitionContext context)
		{
			qualifiers.RemoveAt(qualifiers.Count - 1);
		}

		public override void EnterTypeDefinition(CovaParser.TypeDefinitionContext context)
		{
			qualifiers.Add(context.identifier());
			definition.FindAncestor<IHasTypes>()?.Types.Add(
				type = new Type {
					Name = CurrentQualifier,
					Module = definition.FindAncestor<Module>(),
					Parent = type,
					Location = context.ToTextSourceSpan()
				}
			);
		}

		public override void ExitTypeDefinition(CovaParser.TypeDefinitionContext context)
		{
			qualifiers.RemoveAt(qualifiers.Count - 1);
			type = type.FindAncestorOrNull<Type>();
		}

		public override void EnterFunctionDefinition(CovaParser.FunctionDefinitionContext context)
		{
			qualifiers.Add(context.identifier());
			definition.FindAncestor<IHasFunctions>()?.Functions.Add(
				function = new Function {
					Name = CurrentQualifier,
					Module = definition.FindAncestor<Module>(),
					Parent = type,
					Location = context.ToTextSourceSpan()
				}
			);
		}

		public override void ExitFunctionDefinition(CovaParser.FunctionDefinitionContext context)
		{
			qualifiers.RemoveAt(qualifiers.Count - 1);
			function = function.FindAncestorOrNull<Function>();
		}

		//public override void EnterLocalDefinition(LocalDefinitionContext context)
		//{
		//}

		//public override void EnterSequenceExpression(SequenceExpressionContext context)
		//{
		//	var expressions = context.expression();
		//	var lower = expressions[0];
		//	var upper = expressions[1];
		//	var interval = expressions[3];
		//}
	}
}