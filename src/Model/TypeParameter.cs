namespace Cova.Model
{
    public class TypeParameter : Symbol
    {
        protected TypeParameter() {}

        public TypeParameter(DefinitionSource definitionSource, TypeReference typeReference) : base(definitionSource) =>
            TypeReference = typeReference;
        public TypeReference TypeReference { get; set; } = null!;
    }
}