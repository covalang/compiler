using System;

namespace Cova.Model
{
    public abstract class SymbolNamed : Symbol, IHasName
    {
        protected SymbolNamed() {}
        protected SymbolNamed(DefinitionSource definitionSource, String name) : base(definitionSource) => Name = name;
        public String Name { get; set; } = null!;
    }
}