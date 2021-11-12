namespace Cova.Model
{
    public interface IType :
        IScope,
        IHasName,
        IHasTypeParameters,
        IHasTypes,
        IHasFunctions,
        IHasFields,
        IHasProperties,
        IStorageReferencing
    { }
}