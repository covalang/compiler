using Antlr4.Runtime.Misc;
using Cova.Compiler.Parser.Grammar;
using Cova.Symbols;
using System;

using static Cova.Compiler.Parser.Grammar.CovaParser;

namespace Cova
{
	class SymbolRegistrationVisitor : CovaParserBaseVisitor<Byte>
	{
		private ISymbol symbol;
		private readonly SymbolResolver symbolResolver = new();
		public SymbolRegistrationVisitor(IModule rootSymbol) => symbol = rootSymbol;

		private static I CreateInstance<I>() where I : class => InterfaceImplementor.CreateAndInitialize<I>();

		readonly ref struct SymbolHolder
		{
			readonly SymbolRegistrationVisitor visitor;
			readonly ISymbol symbol;
			public SymbolHolder(SymbolRegistrationVisitor visitor) => (this.visitor, this.symbol) = (visitor, visitor.symbol);
			public void Dispose() => visitor.symbol = this.symbol;
		}

		public override Byte VisitNamespaceDefinition(NamespaceDefinitionContext context)
		{
			using var holder = new SymbolHolder(this);
			foreach (var identifier in context.qualifiedIdentifier().identifier())
			{
				var @namespace = CreateInstance<INamespace>();
				@namespace.DefinitionSource = context.ToTextSourceSpan();
				@namespace.Parent = symbol;
				@namespace.Name = identifier.GetText();
				(symbol as IHasNamespaces)?.Namespaces.Add(@namespace);
				symbol.Children.Add(@namespace);
				symbolResolver.TryRegister(@namespace, out _);
				symbol = @namespace;
			}
			return base.VisitNamespaceDefinition(context);
		}

		public override byte VisitTypeDefinition(TypeDefinitionContext context)
		{
			using var holder = new SymbolHolder(this);
			
			var type = CreateInstance<IType>();
			type.DefinitionSource = context.ToTextSourceSpan();
			type.Name = context.identifier().GetText();
			type.Parent = symbol;
			(symbol as IHasTypes)?.Types.Add(type);
			symbol.Children.Add(type);
			symbolResolver.TryRegister(type, out _);
			symbol = type;

			return base.VisitTypeDefinition(context);
		}

		public override byte VisitFieldDefinition(FieldDefinitionContext context)
		{
			using var holder = new SymbolHolder(this);
			
			var field = CreateInstance<IField>();
			field.DefinitionSource = context.ToTextSourceSpan();
			field.Name = context.identifier().GetText();
			field.Parent = symbol;
			(symbol as IHasFields)?.Fields.Add(field);
			symbol.Children.Add(field);
			symbolResolver.TryRegister(field, out _);
			symbol = field;
			
			return base.VisitFieldDefinition(context);
		}

		public override byte VisitPropertyDefinition(PropertyDefinitionContext context)
		{
			using var holder = new SymbolHolder(this);

			var property = CreateInstance<IProperty>();
			property.DefinitionSource = context.ToTextSourceSpan();
			property.Name = context.identifier().GetText();
			property.Parent = symbol;
			(symbol as IHasProperties)?.Properties.Add(property);
			symbol.Children.Add(property);
			symbolResolver.TryRegister(property, out _);
			symbol = property;

			return base.VisitPropertyDefinition(context);
		}

		public override byte VisitFunctionDefinition(FunctionDefinitionContext context)
		{
			using var holder = new SymbolHolder(this);

			var function = CreateInstance<IFunction>();
			function.DefinitionSource = context.ToTextSourceSpan();
			function.Name = context.identifier().GetText();
			function.Parent = symbol;
			(symbol as IHasFunctions)?.Functions.Add(function);
			symbol.Children.Add(function);
			symbolResolver.TryRegister(function, out _);
			symbol = function;

			return base.VisitFunctionDefinition(context);
		}

		public override byte VisitLocalDeclaration(LocalDeclarationContext context)
		{
			using var holder = new SymbolHolder(this);

			var local = CreateInstance<ILocal>();
			local.DefinitionSource = context.ToTextSourceSpan();
			local.Name = context.identifier().GetText();
			local.Parent = symbol;
			(symbol as IHasLocals)?.Locals.Add(local);
			symbol.Children.Add(local);
			symbolResolver.TryRegister(local, out _);
			symbol = local;

			return base.VisitLocalDeclaration(context);
		}
	}
}
