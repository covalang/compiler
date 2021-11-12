namespace Cova.Model
{
    public interface INamespace : ISymbol, IScope, IHasName, IHasNamespaces, IHasTypes, IHasAliases, IHasFunctions {}
}