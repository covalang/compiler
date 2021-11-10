using System.Collections.Generic;

namespace Cova.Model
{
    public class Scope : Symbol
    {
        protected Scope() {}
        public Scope(DefinitionSource definitionSource) : base(definitionSource) {}
        public List<Scope> Imported { get; } = new();
    }
}