namespace Cova.Model
{
    public interface IStorageReferencing
        : IHasOwnership
            , IHasVisibility
            , IHasMutability
            , IHasNullability
            , IHasStorageType
            , IHasCyclePossibility
            , IHasInstanceDependency
            , IHasThreadShareability
    { }
}