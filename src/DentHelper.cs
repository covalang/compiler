using System;
using Antlr4.Runtime;

public sealed class DentHelper
{
	private readonly Int32 newlineToken;
	private readonly Int32 indentationToken;
	private readonly Int32 indentToken;
	private readonly Int32 dedentToken;
	public DentHelper(Int32 newlineToken, Int32 indentationToken, Int32 indentToken, Int32 dedentToken)
	{
		this.newlineToken = newlineToken;
		this.indentationToken = indentationToken;
		this.indentToken = indentToken;
		this.dedentToken = dedentToken;
	}

	private UInt32 currentDentLevel;
	private Boolean anyNewlineSinceLastNonTab;
	private UInt32 tabCount;
	private IToken? currentToken;

	public IToken NextTokenWithIndentation(Func<IToken> baseNextToken)
	{
		if (currentToken == null)
			currentToken = baseNextToken();

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
		}
		else
		{
			if (anyNewlineSinceLastNonTab)
			{
				if (tabCount != 0)
				{
					if (tabCount != currentDentLevel)
						return GetDentToken();
					else
					{
						anyNewlineSinceLastNonTab = false;
						tabCount = 0;
					}
				}
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
			return CreateToken(indentToken, "Indent");
		}
		else if (tabCount < currentDentLevel) {
			--currentDentLevel;
			return CreateToken(dedentToken, "Dedent");
		}
		else
			throw new InvalidOperationException($"The value of `{nameof(tabCount)}` cannot be equal to `{nameof(currentDentLevel)}` here.");
	}

	private CommonToken CreateToken(Int32 type, String name) =>
		new CommonToken(type, "<" + name + ">")
		{
			Line = currentToken.Line,
			Column = currentToken.Column,
			StartIndex = currentToken.StopIndex,
			StopIndex = currentToken.StopIndex
		};
}