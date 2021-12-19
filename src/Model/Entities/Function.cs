using System;
using System.Collections.Generic;

namespace Cova.Model
{
    public sealed class Function :
        SymbolNamedStorageReferencing,
        IHasChildren<TypeParameter>,
        IHasChildren<Parameter>,
        IHasChildren<Local>,
        IHasChildren<Statement>
    {
        private Function() {}
        public Function(DefinitionSource definitionSource, String name, QualifiedSymbolReference returnType) : base(definitionSource, name) => ReturnType = returnType;

        public QualifiedSymbolReference ReturnType { get; set; } = null!;
        public List<TypeParameter> TypeParameters { get; } = new();
        public List<Parameter> Parameters { get; } = new();
        public List<Local> Locals { get; } = new();
        public List<Statement> Statements { get; } = new();
        
        ICollection<TypeParameter> IHasChildren<TypeParameter>.Children => TypeParameters;
        ICollection<Parameter> IHasChildren<Parameter>.Children => Parameters;
        ICollection<Local> IHasChildren<Local>.Children => Locals;
        ICollection<Statement> IHasChildren<Statement>.Children => Statements;
    }
}