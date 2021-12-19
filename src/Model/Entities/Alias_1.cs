namespace Cova.Model
{
    public abstract class Alias<TSymbol> : Symbol where TSymbol : class 
    {
        protected Alias() {}
        protected Alias(DefinitionSource definitionSource, TSymbol aliased) : base(definitionSource) => Aliased = aliased;

        public TSymbol Aliased { get; private set; } = null!;
    }
}