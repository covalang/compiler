using System;

namespace Cova.Model
{
    public abstract class NamedScope : Scope
    {
        protected NamedScope() {}
        protected NamedScope(DefinitionSource definitionSource, String name) : base(definitionSource) => Name = name;
        public String Name { get; set; } = null!;
    }
}