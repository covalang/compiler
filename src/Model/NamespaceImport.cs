namespace Cova.Model
{
    public class NamespaceImport : Alias<Namespace>
    {
        protected NamespaceImport() {}
        public NamespaceImport(DefinitionSource definitionSource, Namespace aliased) : base(definitionSource, aliased) {}
    }
}