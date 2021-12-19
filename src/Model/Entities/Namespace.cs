using System;
using System.Collections.Generic;

namespace Cova.Model
{
    public sealed class Namespace :
        ScopeNamed,
        IHasChildren<Type>,
        IHasChildren<Function>,
        IHasChildren<Namespace>,
        IHasChildren<Alias>
    {
        private Namespace() {}
        public Namespace(DefinitionSource definitionSource, String name) : base(definitionSource, name) { }
        public Namespace? ParentNamespace { get; set; }
        public Int64? ParentNamespaceId { get; set; }

        public List<DefinitionSource> DefinitionSources { get; } = new();
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