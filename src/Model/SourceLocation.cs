using System;

namespace Cova.Model
{
    public sealed class SourceLocation : EntityBase
    {
        private SourceLocation() {}
        public SourceLocation(String path) => Path = path;
        public String Path { get; set; } = null!;
        public UInt32 Offset { get; set; }
        public UInt32 Line { get; set; }
        public UInt32 Column { get; set; }
    }
}