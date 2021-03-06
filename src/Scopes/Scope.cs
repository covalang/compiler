using System;
using System.Collections.Generic;
using Cova.Symbols;

namespace Cova.Scopes
{
	abstract class ScopeBase : IScopeBase
	{
		public HashSet<IScope> Children { get; init; } = new();
		public HashSet<IScope> Imported { get; init; } = new();
		public HashSet<ISymbol> Symbols { get; init; } = new();
	}

	sealed class RootScope : ScopeBase { }

	abstract class Scope : ScopeBase
	{
		public ScopeBase Parent { get; init; }

		protected Scope(ScopeBase parent) => Parent = parent;
	}

	sealed class AnonymousScope : Scope
	{
		public AnonymousScope(ScopeBase parent) : base(parent) { }
	}

	abstract class NamedScope : Scope
	{
		public abstract String Name { get; protected set; }
		public NamedScope(ScopeBase parent) : base(parent) { }
	}

	sealed class FileScope : NamedScope
	{
		public override String Name { get; protected set; }
		public FileScope(String name, ScopeBase parent) : base(parent) => Name = name;
	}
}