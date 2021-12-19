using System;

namespace Cova.Model
{
    public sealed class Field : SymbolNamedStorageReferencing
    {
        private Field() {}
        public Field(DefinitionSource definitionSource, String name, TypeReference typeReference) : base(definitionSource, name)
        {
            TypeReference = typeReference;
        }

        public TypeReference TypeReference { get; set; } = null!;
    }
}