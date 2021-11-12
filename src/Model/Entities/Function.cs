using System;
using System.Collections.Generic;

namespace Cova.Model
{
    public sealed class Function : NamedScope
    {
        private Function() {}
        public Function(DefinitionSource definitionSource, String name, String typeReference) : base(definitionSource, name) => TypeReference = typeReference;

        public String TypeReference { get; set; } = null!;
        public Ownership Ownership { get; set; }
        public Visibility Visibility { get; set; }
        public Mutability Mutability { get; set; }
        public Nullability Nullability { get; set; }
        public StorageType StorageType { get; set; }
        public CyclePossibility CyclePossibility { get; set; }
        public InstanceDependency InstanceDependency { get; set; }
        public ThreadShareability ThreadShareability { get; set; }
        public List<Parameter> Parameters { get; } = new();
        public List<Local> Locals { get; } = new();
        public List<Statement> Statements { get; } = new();
    }
}