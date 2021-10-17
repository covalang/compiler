using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Cova.Compiler.Parser.Grammar;
using Cova.Symbols;

using static Cova.Compiler.Parser.Grammar.CovaParser;

namespace Cova
{
	class SymbolRegistrationListener : CovaParserBaseListener
	{
		private const Char QualifierDelimiter = '.';

		private readonly SymbolResolver symbolResolver = new();
		private ISymbol symbol;	

		public SymbolRegistrationListener(ISymbol rootSymbol) => symbol = rootSymbol;

		//public override void EnterEveryRule([NotNull] ParserRuleContext context) => 

		private readonly List<ParserRuleContext> qualifiers = new();
		private String CurrentQualifier => String.Join(QualifierDelimiter, CurrentQualifiers.Select(x => x.GetText()));

		private IEnumerable<IdentifierContext> CurrentQualifiers => qualifiers
			.SelectMany(
				x => x switch
				{
					QualifiedIdentifierContext qic => qic.identifier(),
					IdentifierContext ic => new[] { ic },
					_ => throw new InvalidOperationException()
				}
			);

		public override void EnterNamespaceDefinition(NamespaceDefinitionContext context)
		{
			qualifiers.AddRange(context.qualifiedIdentifier().identifier());
			
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
		}

		public override void ExitNamespaceDefinition(NamespaceDefinitionContext context)
		{
			qualifiers.RemoveAt(qualifiers.Count - 1);

			while (symbol.DefinitionSource == context.ToTextSourceSpan())
				symbol = symbol.Parent!;
		}

		public override void EnterTypeDefinition(TypeDefinitionContext context)
		{
			qualifiers.Add(context.identifier());

			var type = InterfaceImplementor.CreateAndInitialize<IType>();
			type.DefinitionSource = context.ToTextSourceSpan();
			type.Parent = symbol;
			type.Name = context.identifier().GetText();
			(symbol as IHasTypes)?.Types.Add(type);
			symbol.Children.Add(type);
			if (!symbolResolver.TryRegister(type, out _))
				Console.WriteLine("Duplicate symbol");
			symbol = type;
		}

		public override void ExitTypeDefinition(TypeDefinitionContext context)
		{
			qualifiers.RemoveAt(qualifiers.Count - 1);

			symbol = symbol.Parent!;
		}

		public override void EnterFunctionDefinition(FunctionDefinitionContext context)
		{
			qualifiers.Add(context.identifier());

			var function = InterfaceImplementor.CreateAndInitialize<IFunction>();
			function.DefinitionSource = context.ToTextSourceSpan();
			function.Parent = symbol;
			(symbol as IHasFunctions)?.Functions.Add(function);
			symbol.Children.Add(function);
			function.Name = context.identifier().GetText();
			if (!symbolResolver.TryRegister(function, out _))
				Console.WriteLine("Duplicate symbol");
			symbol = function;
		}

		public override void ExitFunctionDefinition(FunctionDefinitionContext context)
		{
			qualifiers.RemoveAt(qualifiers.Count - 1);

			symbol = symbol.Parent!;
		}

		public override void EnterFieldDefinition(FieldDefinitionContext context)
		{
			var field = InterfaceImplementor.CreateAndInitialize<IField>();
			field.DefinitionSource = context.ToTextSourceSpan();
			field.Parent = symbol;
			field.Name = context.identifier().GetText();
			if (!symbolResolver.TryRegister(field, out _))
				Console.WriteLine("Duplicate symbol");
			//var localScope = field.FindAncestor<IScope>();
			//var typeCandidates = 
			//field.Type = symbolResolver.Resolve(localScope, context.memberType().GetText());
		}

		//public override void ExitFieldDefinition([NotNull] FieldDefinitionContext context)
		//{
		//	symbol = symbol.Parent!;
		//}

		public override void EnterPropertyDefinition(PropertyDefinitionContext context)
		{
			var property = InterfaceImplementor.CreateAndInitialize<IProperty>();
			property.DefinitionSource = context.ToTextSourceSpan();
			property.Parent = symbol;
			property.Name = context.identifier().GetText();
			if (!symbolResolver.TryRegister(property, out _))
				Console.WriteLine("Duplicate symbol");
		}

		//public override void ExitPropertyDefinition([NotNull] PropertyDefinitionContext context)
		//{
		//	symbol = symbol.Parent!;
		//}

		public override void EnterLocalDeclaration(LocalDeclarationContext context)
		{
			var local = InterfaceImplementor.CreateAndInitialize<ILocal>();
			local.DefinitionSource = context.ToTextSourceSpan();
			local.Parent = symbol;
			local.Name = context.identifier().GetText();
			//if (context.qualifiedIdentifier() is QualifiedIdentifierContext qid)
			//{
			//	var typeReference = InterfaceImplementor.CreateAndInitialize<ITypeReference>();
			//	typeReference.Parent = local;
			//	typeReference.Name = qid.GetText();
			//}
			//else
			//{

			//}
			//symbol = local;
		}

		//public override void ExitLocalDeclaration(LocalDeclarationContext context)
		//{
		//	symbol = symbol.Parent!;
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