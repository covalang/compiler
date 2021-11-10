using System;
using System.Collections.Generic;

namespace Cova.Model
{
    public class Module : NamedScope
    {
        protected Module() {}
        public Module(DefinitionSource definitionSource, String name) : base(definitionSource, name) {}
        
        public List<Type> Types { get; } = new();
        public List<Function> Functions { get; } = new();
        public List<Namespace> Namespaces { get; } = new();
        public List<Alias> Aliases { get; } = new();

    }
}