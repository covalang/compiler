using System;
using System.Collections.Generic;

namespace Cova.Model
{
    public sealed class Parameter : SymbolNamedStorageReferencing
    {
        private Parameter() {}
        public Parameter(DefinitionSource definitionSource, String name, TypeReference typeReference) : base(definitionSource, name)
        {
            TypeReference = typeReference;
        }

        public TypeReference TypeReference { get; set; } = null!;
    }
}