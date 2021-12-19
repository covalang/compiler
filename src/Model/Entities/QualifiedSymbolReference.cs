using System;
using System.Collections.Generic;
using System.Linq;

namespace Cova.Model
{
    public class QualifiedSymbolReference : EntityBase, IEquatable<QualifiedSymbolReference>
    {
        public List<SymbolReference> SymbolReferences { get; } = new();

        public override Boolean Equals(Object? obj) => Equals(obj as QualifiedSymbolReference);
        
        public Boolean Equals(QualifiedSymbolReference? other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return SymbolReferences.SequenceEqual(other.SymbolReferences);
        }


        public override Int32 GetHashCode()
        {
            var hashCode = new HashCode();
            foreach (var sr in SymbolReferences)
                hashCode.Add(sr);
            return hashCode.ToHashCode();
        }
    }
}