using System;
using System.Collections.Generic;

namespace Cova.Model
{
    public abstract class Scope : Symbol, IHasChildren<QualifiedSymbolReference>
    {
        protected Scope() {}
        public Scope(DefinitionSource definitionSource) : base(definitionSource) {}
        public List<QualifiedSymbolReference> Imported { get; } = new();

        ICollection<QualifiedSymbolReference> IHasChildren<QualifiedSymbolReference>.Children => Imported;
    }
}