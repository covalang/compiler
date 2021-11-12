namespace Cova.Model
{
    public interface IClass : IType, IExtends<IClass>, IImplements<IInterface> {}
}