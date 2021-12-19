using System.Collections.Generic;

namespace Cova.Model
{
    public interface IHasChildren<TChild> { ICollection<TChild> Children { get; } }
}