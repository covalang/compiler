using System;

namespace Cova.Model
{
    public sealed class Property : Symbol
    {
        private Property() {}
        public Property(DefinitionSource definitionSource, String name) : base(definitionSource) => Name = name;

        public String Name { get; set; } = null!;
        public Function? Getter { get; set; }
        public Function? Setter { get; set; }
    }
}