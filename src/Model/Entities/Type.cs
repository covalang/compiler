using System;
using System.Collections.Generic;

namespace Cova.Model
{
    public sealed class Type : NamedScope
    {
        private Type() {}
        public Type(DefinitionSource definitionSource, String name) : base(definitionSource, name) {}
        public Ownership Ownership { get; set; }
        public Visibility Visibility { get; set; }
        public Mutability Mutability { get; set; }
        public Nullability Nullability { get; set; }
        public StorageType StorageType { get; set; }
        public CyclePossibility CyclePossibility { get; set; }
        public InstanceDependency InstanceDependency { get; set; }
        public ThreadShareability ThreadShareability { get; set; }

        public List<TypeParameter> TypeParameters { get; } = new();
        public List<Type> Types { get; } = new();
        public List<Function> Functions { get; } = new();
        public List<Field> Fields { get; } = new();
        public List<Property> Properties { get; } = new();
    }
}