namespace Cova.Model
{
    public interface IProperty : ISymbol, IHasName
    {
        IFunction Getter { get; set; }
        IFunction Setter { get; set; }
    }
}