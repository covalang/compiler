using System;
using System.Collections.Generic;

namespace Cova.Model
{
    public sealed class Namespace : NamedScope
    {
        private Namespace() {}
        public Namespace(DefinitionSource definitionSource, String name) : base(definitionSource, name) { }
        public List<Namespace> Namespaces { get; } = new();
        public List<Alias> Aliases { get; } = new();
        public List<Type> Types { get; } = new();
        public List<Function> Functions { get; } = new();
    }
}