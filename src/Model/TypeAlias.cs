using System;

namespace Cova.Model
{
    public class TypeAlias : Alias<Type>
    {
        protected TypeAlias() {}
        public TypeAlias(DefinitionSource definitionSource, Type aliased, String name) : base(definitionSource, aliased) {}
        public String Name { get; set; } = null!;
    }
}