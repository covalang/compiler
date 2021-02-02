using System;
using System.Collections.Generic;

namespace Cova.Scopes
{
	interface IReadOnlyScopeBase
	{
		IReadOnlySet<IScope> Children { get; }
		IReadOnlySet<IScope> Imported { get; }
		IReadOnlySet<Symbol> Symbols { get; }
	}
	
	interface IScopeBase : IReadOnlyScopeBase
	{
		new HashSet<IScope> Children { get; }
		new HashSet<IScope> Imported { get; }
		new HashSet<Symbol> Symbols { get; }

		IReadOnlySet<IScope> IReadOnlyScopeBase.Children => Children;
		IReadOnlySet<IScope> IReadOnlyScopeBase.Imported => Imported;
		IReadOnlySet<Symbol> IReadOnlyScopeBase.Symbols => Symbols;
	}

	interface IRootScope : IScopeBase { }

	interface IReadOnlyScope : IReadOnlyScopeBase
	{
		IReadOnlyScopeBase Parent { get; }
	}
	interface IScope : IReadOnlyScope, IScopeBase
	{
		new IScopeBase Parent { get; }
		IReadOnlyScopeBase IReadOnlyScope.Parent => Parent;
	}

	interface IAnonymousScope : IScope { }

	interface INamedScope : IScope
	{
		String Name { get; }
	}

	// interface IFileScope : INamedScope
	// {
	// 	new IRootScope Parent { get; }
	// }
}
