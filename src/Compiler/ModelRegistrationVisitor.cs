// using System;
// using System.Linq;
// using Cova.Model;
// using Cova.Compiler.Parser.Grammar;
// using static Cova.Compiler.Parser.Grammar.CovaParser;
//
// namespace Cova
// {
// 	public class ModelRegistrationVisitor : CovaParserBaseVisitor<Byte>
// 	{
// 		private readonly SymbolHolder symbolHolder;
// 		private readonly Context db;
// 		public ModelRegistrationVisitor(Module module, Context db) => (symbolHolder, this.db) = (new (module), db);
//
// 		public override Byte VisitUseNamespaceStatement(UseNamespaceStatementContext context)
// 		{
// 			using var parent = symbolHolder.GetParent();
// 			if (parent.Symbol is not Scope scope)
// 				throw new InvalidOperationException();
// 			var qfs = new QualifiedSymbolReference();
// 			var identifiers = context.qualifiedIdentifier().identifier().Select(x => x.GetText());
// 			qfs.SymbolReferences.AddRange(identifiers.Select(x => new SymbolReference(x)));
// 			scope.Imported.Add(qfs);
// 			return base.VisitUseNamespaceStatement(context);
// 		}
//
// 		public override Byte VisitNamespaceDefinition(NamespaceDefinitionContext context)
// 		{
// 			using var parent = symbolHolder.GetParent();
// 			foreach (var identifier in context.qualifiedIdentifier().identifier())
// 			{
// 				var @namespace = new Namespace(context.ToDefinitionSource(), identifier.GetText())
// 				{
// 					Parent = parent.Symbol
// 				};
// 			}
// 			return base.VisitNamespaceDefinition(context);
// 		}
//
// 		class SymbolHolder
// 		{
// 			private Symbol symbol;
// 			public SymbolHolder(Symbol symbol) => this.symbol = symbol;
// 			public ParentHolder GetParent() => new(this);
//
// 			public readonly ref struct ParentHolder
// 			{
// 				private readonly SymbolHolder holder;
// 				public readonly Symbol Symbol;
// 				public ParentHolder(SymbolHolder holder) => (this.holder, Symbol) = (holder, holder.symbol);
// 				public void Dispose() => holder.symbol = Symbol;
// 			}
// 		}
// 	}
// }