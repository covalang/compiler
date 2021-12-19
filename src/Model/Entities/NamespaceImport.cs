namespace Cova.Model
{
    public sealed class NamespaceImport : Symbol
    {
        private NamespaceImport() {}
        public NamespaceImport(DefinitionSource definitionSource, QualifiedSymbolReference symbolReference) : base(definitionSource) => SymbolReference = symbolReference;

        public QualifiedSymbolReference SymbolReference { get; set; } = null!;
    }
}