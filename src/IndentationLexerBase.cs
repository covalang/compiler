using System;
using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;

public abstract class IndentationLexerBase : Antlr4.Runtime.Lexer
{
	// Initializing `pendingDent` to true means any whitespace at the beginning
	// of the file will trigger an INDENT, which will probably be a syntax error,
	// as it is in Python.
	private bool pendingDent = true;
	private int indentCount = 0;
	private readonly Queue<IToken> tokenQueue = new Queue<IToken>();
	private readonly Stack<int> indentStack = new Stack<int>();
	private IToken? initialIndentToken = null;
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

	protected IndentationLexerBase(ICharStream input) : base(input) {}
	protected IndentationLexerBase(ICharStream input, TextWriter output, TextWriter errorOutput) : base(input, output, errorOutput) {}
	
	protected IToken? NextTokenWithIndentation(int newLineToken, int indentToken, int dedentToken)
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
		if (pendingDent && null == initialIndentToken && newLineToken != next.Type)
			initialIndentToken = next;
			
		if (null == next || Hidden == next.Channel || newLineToken == next.Type)
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
				tokenQueue.Enqueue(createToken(newLineToken, "NEWLINE", next));
			}
		}

		// Before exiting `pendingDent` state queue up proper INDENTS and DEDENTS.
		while (indentCount != SavedIndent)
		{
			if (indentCount > SavedIndent)
			{
				indentStack.Push(indentCount);
				tokenQueue.Enqueue(createToken(indentToken, "INDENT" + (indentCount - 1), next));
			}
			else
			{
				indentStack.Pop();
				tokenQueue.Enqueue(createToken(dedentToken, "DEDENT" + SavedIndent, next));
			}
		}
		pendingDent = false;
		tokenQueue.Enqueue(next);
		return tokenQueue.Dequeue();
	}

	protected void FoundNewLine(int hiddenChannel) {
		if (pendingDent)
			Channel = hiddenChannel;
		pendingDent = true;
		indentCount = 0;
		initialIndentToken = null;
	}

	protected void FoundIndentations(int hiddenChannel) {
		Channel = hiddenChannel;
		if (pendingDent)
			indentCount += Text.Length;
	}
}