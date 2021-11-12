using System;

namespace Cova.Model
{
    public sealed class Parameter : Symbol
    {
        private Parameter() {}
        public Parameter(DefinitionSource definitionSource, String name, TypeReference typeReference) : base(definitionSource)
        {
            Name = name;
            TypeReference = typeReference;
        }

        public String Name { get; set; } = null!;
        public TypeReference TypeReference { get; set; } = null!;
        public Ownership Ownership { get; set; }
        public Visibility Visibility { get; set; }
        public Mutability Mutability { get; set; }
        public Nullability Nullability { get; set; }
        public StorageType StorageType { get; set; }
        public CyclePossibility CyclePossibility { get; set; }
        public InstanceDependency InstanceDependency { get; set; }
        public ThreadShareability ThreadShareability { get; set; }
    }
}