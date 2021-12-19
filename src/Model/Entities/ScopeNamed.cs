using System;

namespace Cova.Model
{
    public abstract class ScopeNamed : Scope, IHasName
    {
        protected ScopeNamed() {}
        protected ScopeNamed(DefinitionSource definitionSource, String name) : base(definitionSource) => Name = name;
        public String Name { get; set; } = null!;
    }
}