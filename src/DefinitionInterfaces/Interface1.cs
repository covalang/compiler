using System;
using System.Collections.Generic;

namespace Compiler.DefinitionInterfaces
{
	interface INamespaceDefinition
	{
		public IEnumerable<String> Names { get; }
	}
}
