using System;

namespace Cova.Model
{
    public sealed class Local : SymbolNamedStorageReferencing
    {
        private Local() {}
        public Local(DefinitionSource definitionSource, String name) : base(definitionSource, name) {}
    }
}