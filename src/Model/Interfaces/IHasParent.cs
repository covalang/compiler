namespace Cova.Model
{
    public interface IHasParent<TParent> { TParent Parent { get; set; } }
}