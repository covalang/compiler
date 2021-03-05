using System;

namespace Compiler.Symbols
{
	public abstract record DefinitionSource { private protected DefinitionSource() {} }
	public sealed record TextSourceSpan(TextSourceLocation Begin, TextSourceLocation End) : DefinitionSource;
	public sealed record TextFileSourceSpan(TextFileSourceLocation Begin, TextFileSourceLocation End) : DefinitionSource;
	public sealed record MeadataFileSourceSpan(MeadataFileSourceLocation Begin, MeadataFileSourceLocation End) : DefinitionSource;

	public struct TextSourceLocation
	{
		public ReadOnlyMemory<Char> Text;
		public UInt32 Offset;
		public UInt32 Line;
		public UInt32 Column;
	}

	public struct TextFileSourceLocation
	{
		public String Path;
		public UInt32 Offset;
		public UInt32 Line;
		public UInt32 Column;
	}
	
	public struct MeadataFileSourceLocation
	{
		public String Path;
		public UInt64 BitOffset;
	}
}