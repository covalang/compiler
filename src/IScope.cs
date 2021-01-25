using System;
using System.Collections.Generic;

namespace Cova
{
	interface IScopeBase
	{
		IReadOnlySet<IScope> Children { get; }
		IReadOnlySet<IScope> Imported { get; }
	}

	interface IRootScope : IScopeBase
	{
	}

	interface IScope : IScopeBase
	{
		IScopeBase Parent { get; }
	}

	interface IAnonymousScope : IScope { }

	interface INamedScope : IScope
	{
		String Name { get; }
	}

	interface IFileScope : INamedScope
	{
		new IRootScope Parent { get; }
	}

	abstract class ScopeBase
	{
		private readonly Dictionary<String, Symbol> symbols = new Dictionary<String, Symbol>();
		public HashSet<Scope> Children { get; } = new HashSet<Scope>();
		public IReadOnlyDictionary<String, Symbol> Symbols => symbols;
	}

	sealed class RootScope : ScopeBase { }

	abstract class Scope : ScopeBase
	{
		public Scope Parent { get; }
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
