using System;

namespace Cova.Model
{
    public abstract class SymbolNamedStorageReferencing : SymbolNamed, IStorageReferencing
    {
        protected SymbolNamedStorageReferencing() {}
        public SymbolNamedStorageReferencing(DefinitionSource definitionSource, String name) : base(definitionSource, name) {}
            
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