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

	abstract class ScopeBase : IScopeBase
	{
		public HashSet<IScope> Children { get; } = new HashSet<IScope>();
		public HashSet<IScope> Imported { get; } = new HashSet<IScope>();

		private readonly Dictionary<String, Symbol> symbols = new Dictionary<String, Symbol>();
		public IReadOnlyDictionary<String, Symbol> Symbols => symbols;

		IReadOnlySet<IScope> IScopeBase.Children => Children;
		IReadOnlySet<IScope> IScopeBase.Imported => Imported;
	}

	sealed class RootScope : ScopeBase { }

	abstract class Scope : ScopeBase, IScope
	{
		public Scope Parent { get; }
		protected Scope(Scope parent) => Parent = parent;
		IScopeBase IScope.Parent => Parent;
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
