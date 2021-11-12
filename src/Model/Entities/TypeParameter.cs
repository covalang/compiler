namespace Cova.Model
{
    public sealed class TypeParameter : Symbol
    {
        private TypeParameter() {}

        public TypeParameter(DefinitionSource definitionSource, TypeReference typeReference) : base(definitionSource) =>
            TypeReference = typeReference;
        public TypeReference TypeReference { get; set; } = null!;
    }
}