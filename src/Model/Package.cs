using System;
using System.Collections.Generic;

namespace Cova.Model
{
    public class Package : Symbol
    {
        protected Package() {}
        public Package(DefinitionSource definitionSource, String name) : base(definitionSource) {}
        public String Name { get; set; } = null!;
        public List<Module> Modules { get; } = new();
    }
}