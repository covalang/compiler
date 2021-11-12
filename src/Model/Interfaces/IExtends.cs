using System.Collections.Generic;

namespace Cova.Model
{
    public interface IExtends<TType> where TType : IType { List<IType> Extends { get; } }
}