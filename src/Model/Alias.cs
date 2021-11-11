namespace Cova.Model
{
    public sealed class Alias : Alias<Symbol>
    {
        private Alias() {}
        public Alias(DefinitionSource definitionSource, Symbol aliased) : base(definitionSource, aliased) {}
    }
}