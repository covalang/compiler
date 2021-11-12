namespace Cova.Model
{
    public sealed class Statement : Symbol
    {
        private Statement() {}
        public Statement(DefinitionSource definitionSource) : base(definitionSource) {}
    }
}