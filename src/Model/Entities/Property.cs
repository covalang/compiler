using System;

namespace Cova.Model
{
    public sealed class Property : SymbolNamedStorageReferencing
    {
        private Property() {}
        public Property(DefinitionSource definitionSource, String name) : base(definitionSource, name) {}

        public Function? Getter { get; set; }
        public Function? Setter { get; set; }
    }
}