using System;
using System.Collections.Generic;
using Compiler.Symbols;

namespace Cova.Scopes
{
	interface IScopeBase
	{
		HashSet<IScope> Children { get; init; }
		HashSet<IScope> Imported { get; init; }
		HashSet<ISymbol> Symbols { get; init; }
	}

	interface IRootScope : IScopeBase { }

	interface IScope : IScopeBase
	{
		IScopeBase Parent { get; init; }
	}

	interface IAnonymousScope : IScope { }

	interface INamedScope : IScope
	{
		String Name { get; init; }
	}

	// interface IFileScope : INamedScope
	// {
	// 	new IRootScope Parent { get; }
	// }
}
