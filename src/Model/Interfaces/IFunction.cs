namespace Cova.Model
{
    public interface IFunction : ISymbol, IScope, IHasName, IHasTypeReference, IHasParameters, IHasLocals, IHasStatements, IStorageReferencing { }
}