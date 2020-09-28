using System;
using Antlr4.Runtime;

public sealed class CovaLexerHelper
{
	private readonly DentHelper dentHelper;
	private readonly LinearHelper dentModeLinearHelper;
	private readonly (Int32 begin, Int32 end) braceTokens;
	private readonly LinearHelper braceModeLinearHelper;
	public CovaLexerHelper(
		DentHelper dentHelper,
		LinearHelper dentModeLinearHelper,
		(Int32 begin, Int32 end) braceTokens,
		LinearHelper braceModeLinearHelper
	)
	{
		this.dentHelper = dentHelper;
		this.dentModeLinearHelper = dentModeLinearHelper;
		this.braceTokens = braceTokens;
		this.braceModeLinearHelper = braceModeLinearHelper;

		//this.innerNextToken = dentModeLinearHelper.NextToken(() => dentHelper.NextToken(base.NextToken));
	}

	private IToken? currentToken;
	private Boolean braceMode;
	private UInt32 braceLevel;

	private IToken InnerNextToken(Func<IToken> baseNextToken)
	{
		if (braceMode)
			return braceModeLinearHelper.NextToken(baseNextToken);
		else
			return dentModeLinearHelper.NextToken(() => dentHelper.NextToken(baseNextToken));
	}

	public IToken NextToken(Func<IToken> baseNextToken)
	{
		if (currentToken == null)
			currentToken = InnerNextToken(baseNextToken);

		if (currentToken.Type == braceTokens.begin)
		{
			if (braceLevel == 0)
				braceMode = true;
			braceLevel++;
		}
		else if (currentToken.Type == braceTokens.end)
		{
			braceLevel--;
			if (braceLevel == 0)
				braceMode = false;
		}

		var lastToken = currentToken;
		currentToken = InnerNextToken(baseNextToken);
		return lastToken;
	}
}