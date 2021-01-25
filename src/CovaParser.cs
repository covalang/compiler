using Cova;
using System;
using System.Collections.Generic;

public partial class CovaParser
{
	public partial class FileContext : IFileScope, IFileSymbol
	{
		IRootScope RootScope { get; set; }
		HashSet<IScope> Children { get; } = new HashSet<IScope>();
		HashSet<IScope> Imported { get; } = new HashSet<IScope>();

		public String Name { get; internal set; }

		IRootScope IFileScope.Parent => RootScope;
		IScopeBase IScope.Parent => RootScope;

		IReadOnlySet<IScope> IScopeBase.Children => Children;
		IReadOnlySet<IScope> IScopeBase.Imported => Imported;
	}
}
