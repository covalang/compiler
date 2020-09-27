using System;
using Antlr4.Runtime;

public sealed class DentHelper
{
	private readonly Int32 newlineToken;
	private readonly Int32 indentationToken;
	private readonly (Int32 tokenType, String name) indentToken;
	private readonly (Int32 tokenType, String name) dentToken;
	private readonly (Int32 tokenType, String name) dedentToken;

	public DentHelper(Int32 newlineToken, Int32 indentationToken, (Int32 tokenType, String name) indentToken, (Int32 tokenType, String name) dentToken, (Int32 tokenType, String name) dedentToken)
	{
		this.newlineToken = newlineToken;
		this.indentationToken = indentationToken;
		this.indentToken = indentToken;
		this.dentToken = dentToken;
		this.dedentToken = dedentToken;
	}

	private UInt32 currentDentLevel;
	private Boolean anyNewlineSinceLastNonTab;
	private UInt32 tabCount;
	private IToken? currentToken;
	private Boolean done;

	public IToken NextToken(Func<IToken> baseNextToken)
	{
		if (currentToken == null)
		{
			currentToken = baseNextToken();
			return CreateToken(indentToken.tokenType, indentToken.name);
		}

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
				return CreateToken(dedentToken.tokenType, dedentToken.name);
			}
		}
		else
		{
			if (anyNewlineSinceLastNonTab)
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
			return CreateToken(indentToken.tokenType, indentToken.name);
		}
		else if (tabCount < currentDentLevel) {
			--currentDentLevel;
			return CreateToken(dedentToken.tokenType, dedentToken.name);
		}
		else
			return CreateToken(dentToken.tokenType, dentToken.name);
	}

	private CommonToken CreateToken(Int32 type, String name) =>
		new CommonToken(type, name)
		{
			Line = currentToken!.Line,
			Column = currentToken.Column,
			StartIndex = currentToken.StopIndex,
			StopIndex = currentToken.StopIndex
		};
}