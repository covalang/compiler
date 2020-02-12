lexer grammar CovaLexer;

options {
	language = CSharp;
	//superClass = LexerBase;
}

tokens { INDENT, DEDENT }

@lexer::header {
using System.Collections.Generic;
}

@lexer::members {
	// Initializing `pendingDent` to true means any whitespace at the beginning
	// of the file will trigger an INDENT, which will probably be a syntax error,
	// as it is in Python.
	private bool pendingDent = true;
	private int indentCount = 0;
	private readonly Queue<IToken> tokenQueue = new Queue<IToken>();
	private readonly Stack<int> indentStack = new Stack<int>();
	private IToken initialIndentToken = null;
	private int SavedIndent => indentStack.Count == 0 ? 0 : indentStack.Peek();

	private CommonToken createToken(int type, String text, IToken next)
	{
		var token = new CommonToken(type, text);
		if (null != initialIndentToken) {
			token.StartIndex = initialIndentToken.StartIndex;
			token.Line = initialIndentToken.Line;
			token.Column = initialIndentToken.Column;
			token.StopIndex = next.StartIndex - 1;
		}
		return token;
	}

	public override IToken NextToken()
	{
		// Return tokens from the queue if it is not empty.
		if (tokenQueue.Count != 0)
			return tokenQueue.Dequeue();

		// Grab the next token and if nothing special is needed, simply return it.
		// Initialize `initialIndentToken` if needed.
		IToken next = base.NextToken();

		//NOTE: This could be an appropriate spot to count whitespace or deal with
		//NEWLINES, but it is already handled with custom actions down in the
		//lexer rules.
		if (pendingDent && null == initialIndentToken && NEWLINE != next.Type)
			initialIndentToken = next;
			
		if (null == next || Hidden == next.Channel || NEWLINE == next.Type)
			return next;

		// Handle EOF. In particular, handle an abrupt EOF that comes without an
		// immediately preceding NEWLINE.
		if (next.Type == Eof)
		{
			indentCount = 0;

			// EOF outside of `pendingDent` state means input did not have a final
			// NEWLINE before end of file.
			if (!pendingDent)
			{
				initialIndentToken = next;
				tokenQueue.Enqueue(createToken(NEWLINE, "NEWLINE", next));
			}
		}

		// Before exiting `pendingDent` state queue up proper INDENTS and DEDENTS.
		while (indentCount != SavedIndent)
		{
			if (indentCount > SavedIndent)
			{
				indentStack.Push(indentCount);
				tokenQueue.Enqueue(createToken(INDENT, "INDENT" + (indentCount - 1), next));
			}
			else
			{
				indentStack.Pop();
				tokenQueue.Enqueue(createToken(DEDENT, "DEDENT" + SavedIndent, next));
			}
		}
		pendingDent = false;
		tokenQueue.Enqueue(next);
		return tokenQueue.Dequeue();
	}

}

// Keywords

Use: 'use';
Module: 'module';
Type: 'type';
Func: 'func';
True: 'true';
False: 'false';

Dot: '.';
EqualsSign: '=';

// Lexical rules

Identifier
	: [_A-Za-z][_A-Za-z0-9]+
	//: IdentifierStartCharacter IdentifierPartCharacter*
	;

NEWLINE : ( '\r'? '\n' | '\r' ) {
	if (pendingDent)
		Channel = Hidden;
	pendingDent = true;
	indentCount = 0;
	initialIndentToken = null;
};

Whitespace: ' '+;

Indents: '\t'+ {
	Channel = Hidden;
	if (pendingDent)
		indentCount += Text.Length;
};