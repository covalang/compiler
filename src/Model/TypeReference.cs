using System;
using System.Collections.Generic;

namespace Cova.Model
{
    public sealed class TypeReference : EntityBase
    {
        private TypeReference() {}
        public TypeReference(String typeName) => TypeName = typeName;

        public String TypeName { get; set; } = null!;
        public List<String> TypeParameterNames { get; } = new();
    }
}