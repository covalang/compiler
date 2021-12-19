// using Cova.Compiler.Parser.Grammar;
// using Cova.Symbols;
// using System;
// using System.Linq;
// using System.Reflection;
// using Antlr4.Runtime;
// using Antlr4.Runtime.Tree;
// using static Cova.Compiler.Parser.Grammar.CovaParser;
//
// namespace Cova
// {
// 	class SymbolRegistrationVisitor : CovaParserBaseVisitor<Byte>
// 	{
// 		private ISymbol symbol;
// 		private readonly SymbolResolver symbolResolver = new();
// 		public SymbolRegistrationVisitor(IModule rootSymbol) => symbol = rootSymbol;
//
// 		private TSymbol CreateInstance<TSymbol>(ParserRuleContext context, String? name = null) where TSymbol : class, ISymbol
// 		{
// 			var instance = InterfaceImplementor.CreateAndInitialize<TSymbol>();
// 			instance.DefinitionSource = context.ToTextSourceSpan();
// 			instance.Parent = symbol;
// 			if (instance is IHasName hasName)
// 				hasName.Name = name ?? throw new ArgumentNullException(nameof(name), "This symbol has a name but `null` has been passed.");
// 			symbol.Children.Add(instance);
// 			if (symbol.Parent is IHasChildren<TSymbol> hasChildren)
// 				hasChildren.Children.Add(instance);
// 			var orderedSetType = typeof(OrderedSet<>).MakeGenericType(typeof(TSymbol));
// 			var orderedSetPropertyInfo =
// 				instance.Parent
// 					.GetType()
// 					.GetProperties(BindingFlags.Public | BindingFlags.Instance)
// 					.SingleOrDefault(x => x.PropertyType == orderedSetType);
// 			if (orderedSetPropertyInfo != null && orderedSetPropertyInfo.GetValue(instance.Parent) is OrderedSet<TSymbol> set)
// 				set.Add(instance);
// 			symbolResolver.TryRegister(instance, out _);
// 			symbol = instance;
// 			return instance;
// 		}
//
// 		static class ParentContainerCache<T> where T : ISymbol
// 		{
// 			public static Func<T, OrderedSet<T>>? Getter { get; }
//
// 			static ParentContainerCache()
// 			{
// 				var orderedSetType = typeof(OrderedSet<>).MakeGenericType(typeof(T));
// 				var orderedSetPropertyInfo =
// 					typeof(ISymbol).GetProperty(nameof(ISymbol.Children))!
// 						.GetType()
// 						.GetProperties(BindingFlags.Public | BindingFlags.Instance)
// 						.SingleOrDefault(x => x.PropertyType == orderedSetType);
// 				if (orderedSetPropertyInfo == null)
// 					return;
// 				
// 			}
// 		}
//
// 		readonly ref struct SymbolHolder
// 		{
// 			readonly SymbolRegistrationVisitor visitor;
// 			readonly ISymbol symbol;
// 			public SymbolHolder(SymbolRegistrationVisitor visitor) => (this.visitor, symbol) = (visitor, visitor.symbol);
// 			public void Dispose() => visitor.symbol = symbol;
// 		}
//
// 		public override Byte VisitNamespaceDefinition(NamespaceDefinitionContext context)
// 		{
// 			using var holder = new SymbolHolder(this);
// 			foreach (var identifier in context.qualifiedIdentifier().identifier())
// 			{
// 				var @namespace = CreateInstance<INamespace>(context, identifier.GetText());
// 			}
// 			return base.VisitNamespaceDefinition(context);
// 		}
//
// 		public override byte VisitTypeDefinition(TypeDefinitionContext context)
// 		{
// 			using var holder = new SymbolHolder(this);
// 			var type = CreateInstance<IType>(context, context.identifier().GetText());
//
// 			return base.VisitTypeDefinition(context);
// 		}
//
// 		public override byte VisitFieldDefinition(FieldDefinitionContext context)
// 		{
// 			using var holder = new SymbolHolder(this);
// 			var field = CreateInstance<IField>(context, context.identifier().GetText());
// 			
// 			return base.VisitFieldDefinition(context);
// 		}
//
// 		public override byte VisitPropertyDefinition(PropertyDefinitionContext context)
// 		{
// 			using var holder = new SymbolHolder(this);
// 			var property = CreateInstance<IProperty>(context, context.identifier().GetText());
//
// 			return base.VisitPropertyDefinition(context);
// 		}
//
// 		public override byte VisitFunctionDefinition(FunctionDefinitionContext context)
// 		{
// 			using var holder = new SymbolHolder(this);
// 			var function = CreateInstance<IFunction>(context, context.identifier().GetText());
// 			return base.VisitFunctionDefinition(context);
// 		}
//
// 		public override byte VisitLocalDeclaration(LocalDeclarationContext context)
// 		{
// 			using var holder = new SymbolHolder(this);
// 			var local = CreateInstance<ILocal>(context, context.identifier().GetText());
//
// 			return base.VisitLocalDeclaration(context);
// 		}
//
//
// 		public override byte VisitParameter(ParameterContext context)
// 		{
// 			using var holder = new SymbolHolder(this);
// 			var parameter = CreateInstance<IParameter>(context, context.identifier().GetText());
// 			//parameter.TypeReference = new Reference<IType>(context.type().GetText());
// 			return base.VisitParameter(context);
// 		}
// 	}
// }
