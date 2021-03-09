using System;
using System.Collections.Generic;
using Cova.Symbols;

namespace Compiler.DefinitionInterfaces
{
	interface INamespaceDefinition
	{
		public IEnumerable<String> Names { get; }
		public Visibility Visibility { get; }
	}
}
