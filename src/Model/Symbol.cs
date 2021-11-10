using System.Collections.Generic;

namespace Cova.Model
{
    public class Symbol : EntityBase
    {
        protected Symbol() {}
        public Symbol(DefinitionSource definitionSource) => DefinitionSource = definitionSource;
        
        public Symbol? Parent { get; set; }
        public List<Symbol> Children { get; } = new();
        public DefinitionSource DefinitionSource { get; set; } = null!;
    }
}