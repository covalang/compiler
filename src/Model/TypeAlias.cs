using System;

namespace Cova.Model
{
    public sealed class TypeAlias : Alias<Type>
    {
        private TypeAlias() {}
        public TypeAlias(DefinitionSource definitionSource, Type aliased, String name) : base(definitionSource, aliased) {}
        public String Name { get; set; } = null!;
    }
}