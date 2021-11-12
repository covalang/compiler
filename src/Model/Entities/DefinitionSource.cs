namespace Cova.Model
{
    public sealed class DefinitionSource : EntityBase
    {
        private DefinitionSource() {}
        public DefinitionSource(SourceLocation begin, SourceLocation end)
        {
            Begin = begin;
            End = end;
        }

        public SourceLocation Begin { get; set; } = null!;
        public SourceLocation End { get; set; } = null!;
    }
}