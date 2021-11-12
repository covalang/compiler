using System;

namespace Cova.Model
{
    public sealed class Local : Symbol
    {
        private Local() {}
        public Local(DefinitionSource definitionSource, String name) : base(definitionSource) => Name = name;

        public String Name { get; set; } = null!;
    }
}