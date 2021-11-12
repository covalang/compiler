using System.Collections.Generic;

namespace Cova.Model
{
    public interface IScope : ISymbol
    {
        List<IScope> Imported { get; }
    }
}