using System;
using System.Collections.Generic;

namespace Cova.Model
{
    public abstract class EntityBase// : IEquatable<EntityBase<TKey>>
    {
        public Int64 Id { get; private set; }

        // public Boolean Equals(EntityBase<TKey>? other)
        // {
        //     if (ReferenceEquals(null, other))
        //         return false;
        //     if (ReferenceEquals(this, other))
        //         return true;
        //     return EqualityComparer<TKey>.Default.Equals(Id, other.Id);
        // }
        //
        // public override Boolean Equals(Object? obj) => Equals(obj as EntityBase<TKey>);
        // public override Int32 GetHashCode() => EqualityComparer<TKey>.Default.GetHashCode(Id);
    }
}