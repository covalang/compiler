using System.Collections.Generic;

namespace Cova.Model
{
    public interface IImplements<TType> where TType : IType { List<IType> Implements { get; } }
}