using System;
using System.Collections.Generic;

namespace Cova.Model
{
    public abstract class Symbol : EntityBase, IHasChildren<Symbol>, IEquatable<Symbol>
    {
        protected Symbol() {}
        public Symbol(DefinitionSource definitionSource) => DefinitionSource = definitionSource;
        
        public Int64? ParentId { get; set; }
        public Symbol? Parent { get; set; }
        public List<Symbol> Children { get; } = new();
        public DefinitionSource DefinitionSource { get; private init; } = null!;

        ICollection<Symbol> IHasChildren<Symbol>.Children => Children;

        public Boolean Equals(Symbol? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ParentId == other.ParentId && DefinitionSource.Equals(other.DefinitionSource);
        }

        public override Boolean Equals(Object? obj) => Equals(obj as Symbol);
        public override Int32 GetHashCode() =>
            HashCode.Combine(base.GetHashCode(), Parent?.GetHashCode(), DefinitionSource.GetHashCode());
    }
}