namespace Cova.Model
{
    public sealed class NamespaceImport : Alias<Namespace>
    {
        private NamespaceImport() {}
        public NamespaceImport(DefinitionSource definitionSource, Namespace aliased) : base(definitionSource, aliased) {}
    }
}