using Antlr4.Runtime;
using Cova.Symbols;
using System;

namespace Cova
{
	static class AntlrExtensions
	{
		public static Cova.Model.DefinitionSource ToDefinitionSource(this ParserRuleContext context)
		{
			var stopLength = context.Stop.StopIndex - context.Stop.StartIndex;
			return new Cova.Model.DefinitionSource(
				new Cova.Model.SourceLocation(context.Start.TokenSource.SourceName)
				{
					//Path = context.Start.TokenSource.SourceName,
					Offset = (UInt32)context.Start.StartIndex,
					Line = (UInt32)context.Start.Line,
					Column = (UInt32)context.Start.Column
				},
				new Cova.Model.SourceLocation(context.Stop.TokenSource?.SourceName ?? context.Start.TokenSource.SourceName)
				{
					//Path = context.Stop.TokenSource?.SourceName ?? context.Start.TokenSource.SourceName,
					Offset = (UInt32)context.Stop.StopIndex,
					Line = (UInt32)context.Stop.Line,
					Column = (UInt32)(context.Stop.Column + stopLength),
				}
			);
		}
		public static TextFileSourceSpan ToTextSourceSpan(this ParserRuleContext context)
		{
			var stopLength = context.Stop.StopIndex - context.Stop.StartIndex;
			return new TextFileSourceSpan(
				new TextFileSourceLocation
				{
					Path = context.Start.TokenSource.SourceName,
					Offset = (UInt32)context.Start.StartIndex,
					Line = (UInt32)context.Start.Line,
					Column = (UInt32)context.Start.Column
				},
				new TextFileSourceLocation
				{
					Path = context.Stop.TokenSource?.SourceName ?? context.Start.TokenSource.SourceName,
					Offset = (UInt32)context.Stop.StopIndex,
					Line = (UInt32)context.Stop.Line,
					Column = (UInt32)(context.Stop.Column + stopLength),
				}
			);
		}
	}
}
