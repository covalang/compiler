using System.Collections.Generic;

namespace Cova.Model
{
    public interface IHasChildren<TChild> { List<TChild> Children { get; } }
}