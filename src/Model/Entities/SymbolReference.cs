using System;
using System.Collections.Generic;

namespace Cova.Model
{
    public class SymbolReference : EntityBase
    {
        private SymbolReference() {}
        public SymbolReference(String symbolName) => SymbolName = symbolName;

        public String SymbolName { get; set; } = null!;
        public List<QualifiedSymbolReference> TypeParameters { get; } = new();
        public Symbol? ResolvedSymbol { get; set; }
    }
}