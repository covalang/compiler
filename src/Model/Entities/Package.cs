using System;
using System.Collections.Generic;

namespace Cova.Model
{
    public sealed class Package : SymbolNamed, IHasChildren<Module>
    {
        private Package() {}
        public Package(DefinitionSource definitionSource, String name) : base(definitionSource, name) {}
        public List<Module> Modules { get; } = new();

        ICollection<Module> IHasChildren<Module>.Children => Modules;
    }
}