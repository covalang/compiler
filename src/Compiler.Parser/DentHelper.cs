using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Antlr4.Runtime;

public sealed class DentHelper
{
	private readonly Int32 newlineToken;
	private readonly Int32 indentationToken;
	private readonly (Int32 tokenType, String displayText) indentToken;
	private readonly (Int32 tokenType, String displayText) dentToken;
	private readonly (Int32 tokenType, String displayText) dedentToken;
	private readonly ImmutableDictionary<Int32, Int32> otherBlockTerminals;

	public DentHelper(
		Int32 newlineToken,
		Int32 indentationToken,
		(Int32 tokenType, String displayText) indentToken,
		(Int32 tokenType, String displayText) dentToken,
		(Int32 tokenType, String displayText) dedentToken,
		params (Int32 begin, Int32 end)[] otherBlockTerminals
	)
	{
		this.newlineToken = newlineToken;
		this.indentationToken = indentationToken;
		this.indentToken = indentToken;
		this.dentToken = dentToken;
		this.dedentToken = dedentToken;
		this.otherBlockTerminals = otherBlockTerminals.ToImmutableDictionary(x => x.begin, x => x.end);
		this.otherBlockEnds = new Stack<Int32>(32);
	}

	private UInt32 currentDentLevel;
	private Boolean anyNewlineSinceLastNonTab;
	private UInt32 tabCount;
	private IToken? currentToken;
	private Boolean done;
	private readonly Stack<Int32> otherBlockEnds;

	public IToken NextToken(Func<IToken> baseNextToken)
	{
		if (currentToken == null)
		{
			currentToken = baseNextToken();
			return CreateToken(indentToken.tokenType, indentToken.displayText);
		}
		
		if (otherBlockTerminals.TryGetValue(currentToken.Type, out var end))
		{
			otherBlockEnds.Push(end);
		}
		else if (otherBlockEnds.Count != 0 && otherBlockEnds.Peek() == currentToken.Type)
		{
			otherBlockEnds.Pop();
		}

		if (otherBlockEnds.Count == 0)
		{
			if (currentToken.Type == newlineToken)
				anyNewlineSinceLastNonTab = true;
			else if (currentToken.Type == indentationToken)
			{
				if (anyNewlineSinceLastNonTab)
					++tabCount;
			}
			else if (currentToken.Type == Lexer.Eof)
			{
				if (tabCount != currentDentLevel)
					return GetDentToken();
				if (!done)
				{
					done = true;
					return CreateToken(dedentToken.tokenType, dedentToken.displayText);
				}
			}
			else if (anyNewlineSinceLastNonTab)
			{
				var dentToken = GetDentToken();
				if (tabCount == currentDentLevel)
				{
					anyNewlineSinceLastNonTab = false;
					tabCount = 0;
				}
				return dentToken;
			}
		}

		var lastToken = currentToken;
		currentToken = baseNextToken();
		return lastToken;
	}

	private IToken GetDentToken()
	{
		if (tabCount > currentDentLevel) {
			++currentDentLevel;
			return CreateToken(indentToken.tokenType, indentToken.displayText);
		}
		else if (tabCount < currentDentLevel) {
			--currentDentLevel;
			return CreateToken(dedentToken.tokenType, dedentToken.displayText);
		}
		else
			return CreateToken(dentToken.tokenType, dentToken.displayText);
	}

	private CommonToken CreateToken(Int32 type, String displayText) =>
		new(currentToken ?? throw new InvalidOperationException("The current token must not be null. Something is probably seriously wrong here."))
		{
			Type = type,
			Text = displayText,
			StopIndex = currentToken.StartIndex
		};
}