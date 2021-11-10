using System;

namespace Cova.Model
{
    public class Local : Symbol
    {
        protected Local() {}
        public Local(DefinitionSource definitionSource, String name) : base(definitionSource) => Name = name;

        public String Name { get; set; } = null!;
    }
}