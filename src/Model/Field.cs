using System;

namespace Cova.Model
{
    public sealed class Field : Symbol
    {
        private Field() {}
        public Field(DefinitionSource definitionSource, String name, TypeReference typeReference) : base(definitionSource)
        {
            Name = name;
            TypeReference = typeReference;
        }

        public String Name { get; set; } = null!;
        public TypeReference TypeReference { get; set; } = null!;
    }
}