using System;
using System.Collections.Generic;

namespace Cova.Model
{
    public sealed class Type :
        SymbolNamedStorageReferencing,
        IHasChildren<TypeParameter>,
        IHasChildren<Type>,
        IHasChildren<Function>,
        IHasChildren<Field>,
        IHasChildren<Property>
    {
        private Type() {}
        public Type(DefinitionSource definitionSource, String name) : base(definitionSource, name) {}

        public List<TypeParameter> TypeParameters { get; } = new();
        public List<Type> Types { get; } = new();
        public List<Function> Functions { get; } = new();
        public List<Field> Fields { get; } = new();
        public List<Property> Properties { get; } = new();

        ICollection<TypeParameter> IHasChildren<TypeParameter>.Children => TypeParameters;
        ICollection<Type> IHasChildren<Type>.Children => Types;
        ICollection<Function> IHasChildren<Function>.Children => Functions;
        ICollection<Field> IHasChildren<Field>.Children => Fields;
        ICollection<Property> IHasChildren<Property>.Children => Properties;
    }
}