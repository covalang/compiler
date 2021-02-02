using System;
using System.Collections.Generic;

namespace Cova.Scopes
{
	abstract class ScopeBase : IScopeBase
	{
		public HashSet<IScope> Children { get; } = new();
		public HashSet<IScope> Imported { get; } = new();
		public HashSet<Symbol> Symbols { get; } = new();
	}

	sealed class RootScope : ScopeBase { }

	abstract class Scope : ScopeBase, IScope
	{
		public IScopeBase Parent { get; }
		protected Scope(Scope parent) => Parent = parent;
	}

	sealed class AnonymousScope : Scope
	{
		public AnonymousScope(Scope parent) : base(parent) { }
	}

	abstract class NamedScope : Scope
	{
		public abstract String Name { get; protected set; }
		public NamedScope(Scope parent) : base(parent) { }
	}

	sealed class FileScope : NamedScope
	{
		public override String Name { get; protected set; }
		public FileScope(String name, Scope parent) : base(parent) => Name = name;
	}
}