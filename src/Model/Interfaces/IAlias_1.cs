namespace Cova.Model
{
    public interface IAlias<out TSymbol> : ISymbol where TSymbol : ISymbol
    {
        TSymbol Aliased { get; }
    }
}