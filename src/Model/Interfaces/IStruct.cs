namespace Cova.Model
{
    public interface IStruct : IType, IExtends<IStruct>, IImplements<ITrait> {}
}