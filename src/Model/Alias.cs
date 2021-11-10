namespace Cova.Model
{
    public class Alias : Alias<Symbol>
    {
        protected Alias() {}
        public Alias(DefinitionSource definitionSource, Symbol aliased) : base(definitionSource, aliased) {}
    }
}