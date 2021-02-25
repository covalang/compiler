using Compiler.DefinitionInterfaces;
using Cova;
using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.Symbols;

using static CovaParser;

public partial class CovaParser
{
	public partial class FileContext// : IFileScope, IFileSymbol
	{
		//IRootScope RootScope { get; set; }
		//HashSet<IScope> Children { get; } = new HashSet<IScope>();
		//HashSet<IScope> Imported { get; } = new HashSet<IScope>();

		//public String Name { get; internal set; }

		//IRootScope IFileScope.Parent => RootScope;
		//IScopeBase IScope.Parent => RootScope;

		//IReadOnlySet<IScope> IScopeBase.Children => Children;
		//IReadOnlySet<IScope> IScopeBase.Imported => Imported;
	}

	public partial class NamespaceDefinitionContext : INamespaceDefinition
	{
		public IEnumerable<String> Names => qualifiedIdentifier().identifier().Select(x => x.GetText());
		public Visibility Visibility => visibility().ToVisibilityEnum();
	}
}

static class ParserExtensions
{
	public static Visibility ToVisibilityEnum(this VisibilityContext context) => context?.GetChild(0) switch
	{
		null => Visibility.None,
		PrivateVisibilityContext => Visibility.Private,
		ProtectedVisibilityContext => Visibility.Protected,
		InternalVisibilityContext => Visibility.Internal,
		PublicVisibilityContext => Visibility.Public,
		_ => throw new ArgumentException("Invalid child rule", nameof(context))
	};
}