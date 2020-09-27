using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public sealed class LinearHelper
{
	private readonly Int32 linearBodyBegin;
	private readonly (Int32 tokenType, String name) linearBodyEnd;
	private readonly ImmutableDictionary<Int32, (Int32 end, ImmutableHashSet<Int32> continues)> otherBodyTerminals;
	private readonly Stack<NonLinearBody> linearBodies;
	private IToken? currentToken;

	public LinearHelper(Int32 linearBodyBegin, (Int32 tokenType, String name) linearBodyEnd, params (Int32 begin, Int32 end, Int32[] continues)[] otherBodyTerminals)
	{
		this.linearBodyBegin = linearBodyBegin;
		this.linearBodyEnd = linearBodyEnd;
		this.otherBodyTerminals = otherBodyTerminals.ToImmutableDictionary(x => x.begin, x => (x.end, x.continues.ToImmutableHashSet()));
		linearBodies = new Stack<NonLinearBody>(new[] { new NonLinearBody(Lexer.Eof, otherBodyTerminals.First().continues.ToImmutableHashSet()) });
	}

	private sealed class NonLinearBody
	{
		public NonLinearBody(Int32 end, ImmutableHashSet<Int32> continues) { End = end; Continues = continues; }
		public readonly Int32 End;
		public readonly ImmutableHashSet<Int32> Continues;
		public Int32 LinearBodyLevel;
	}

	public IToken NextToken(Func<IToken> baseNextToken)
	{
		if (currentToken == null)
			currentToken = baseNextToken();

		var linearBody = linearBodies.Peek();
		if (linearBodyBegin == currentToken.Type)
		{
			linearBody.LinearBodyLevel++;
		}
		else if (otherBodyTerminals.TryGetValue(currentToken.Type, out var bodyTerminals))
		{
			linearBodies.Push(new NonLinearBody(bodyTerminals.end, bodyTerminals.continues));
		}
		else if (linearBody.End == currentToken.Type || linearBody.Continues.Contains(currentToken.Type))
		{
			if (linearBody.LinearBodyLevel == 0)
			{
				if (linearBody.End == currentToken.Type)
				{
					linearBodies.Pop();
				}
			}
			else
			{
				linearBody.LinearBodyLevel--;
				return CreateToken();
			}
		}

		var lastToken = currentToken;
		currentToken = baseNextToken();
		return lastToken;
	}

	private CommonToken CreateToken() =>
		new CommonToken(linearBodyEnd.tokenType, linearBodyEnd.name)
		{
			Line = currentToken!.Line,
			Column = currentToken.Column,
			StartIndex = currentToken.StopIndex,
			StopIndex = currentToken.StopIndex
		};
}