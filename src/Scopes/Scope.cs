using System;
using System.Collections.Generic;
using Compiler.Symbols;

namespace Cova.Scopes
{
	abstract class ScopeBase : IScopeBase
	{
		public HashSet<IScope> Children { get; init; } = new();
		public HashSet<IScope> Imported { get; init; } = new();
		public HashSet<ISymbol> Symbols { get; init; } = new();
	}

	sealed class RootScope : ScopeBase { }

	abstract class Scope : ScopeBase, IScope
	{
		public IScopeBase Parent { get; init; }
		protected Scope(IScopeBase parent) => Parent = parent;
	}

	sealed class AnonymousScope : Scope
	{
		public AnonymousScope(IScopeBase parent) : base(parent) { }
	}

	abstract class NamedScope : Scope
	{
		public abstract String Name { get; protected set; }
		public NamedScope(IScopeBase parent) : base(parent) { }
	}

	sealed class FileScope : NamedScope
	{
		public override String Name { get; protected set; }
		public FileScope(String name, IScopeBase parent) : base(parent) => Name = name;
	}
}