using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public sealed class LinearHelper
{
	private readonly Int32 linearBodyBegin;
	private readonly (Int32 tokenType, String name) linearBodyEnd;
	private readonly ImmutableDictionary<Int32, (Int32 end, Int32 @continue)> otherBodyTerminals;
	private readonly Stack<NonLinearBody> linearBodies;
	private IToken? currentToken;

	public LinearHelper(
		Int32 linearBodyBegin,
		(Int32 tokenType, String name) linearBodyEnd,
		(Int32 begin, Int32 @continue, Int32 end) defaultBodyTerminals,
		params (Int32 begin, Int32 @continue, Int32 end)[] otherBodyTerminals
	)
	{
		this.linearBodyBegin = linearBodyBegin;
		this.linearBodyEnd = linearBodyEnd;
		this.otherBodyTerminals = otherBodyTerminals.ToImmutableDictionary(x => x.begin, x => (x.@continue, x.end));
		linearBodies = new Stack<NonLinearBody>(
			new[] { new NonLinearBody(defaultBodyTerminals.@continue, Lexer.Eof) }
		);
	}

	private sealed class NonLinearBody
	{
		public NonLinearBody(Int32 @continue, Int32 end) { Continue = @continue; End = end; }
		public readonly Int32 End;
		public readonly Int32 Continue;
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
			linearBodies.Push(new NonLinearBody(bodyTerminals.end, bodyTerminals.@continue));
		}
		else if (linearBody.Continue == currentToken.Type || linearBody.End == currentToken.Type)
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
		new(currentToken)
		{
			Type = linearBodyEnd.tokenType,
			Text = linearBodyEnd.name,
			StopIndex = currentToken.StartIndex
		};
}