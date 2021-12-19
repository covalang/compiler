using System.Collections.Generic;

namespace Cova.Model
{
    public sealed class CompilationUnit :
        Scope,
        IHasChildren<Type>,
        IHasChildren<Function>,
        IHasChildren<Namespace>,
        IHasChildren<Alias>
    {
        private CompilationUnit() {}
        public CompilationUnit(DefinitionSource definitionSource) : base(definitionSource) {}
        
        public List<Type> Types { get; } = new();
        public List<Function> Functions { get; } = new();
        public List<Namespace> Namespaces { get; } = new();
        public List<Alias> Aliases { get; } = new();

        ICollection<Type> IHasChildren<Type>.Children => Types;
        ICollection<Function> IHasChildren<Function>.Children => Functions;
        ICollection<Namespace> IHasChildren<Namespace>.Children => Namespaces;
        ICollection<Alias> IHasChildren<Alias>.Children => Aliases;
    }
}