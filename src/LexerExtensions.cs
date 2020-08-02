using System;
using System.Runtime.CompilerServices;
using Antlr4.Runtime;

namespace Cova
{
	public static class LexerExtensions
	{
		private static readonly ConditionalWeakTable<Lexer, Object> state = new ConditionalWeakTable<Lexer, Object>();

		public static IToken NextTokenWithIndentation(
			this Lexer lexer,
			Int32 IndentToken,
			Int32 DedentToken,
			Int32 NewlineToken,
			Func<IToken> baseNextToken)
		{
			return baseNextToken();

			//DentState GetState() => (DentState) state.GetValue(lexer, l => new DentState());
		}

		class DentState
		{

		}
	}
}