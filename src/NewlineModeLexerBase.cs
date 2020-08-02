using System;
using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;

public abstract class NewlineModeLexerBase : Antlr4.Runtime.Lexer
{
	protected class SpecialTokenTypes
	{
		public Int32 Newline { get; }
		public (Int32 Indentation, Int32 Indent, Int32 Dedent) Dent { get; }
		public (Int32 Begin, Int32 End) SingleLine { get; }
		public (Int32 Begin, Int32 End) MultiLine { get; }

		public SpecialTokenTypes(Int32 newline, (Int32 Indentation, Int32 Indent, Int32 Dedent) dent, (Int32 Begin, Int32 End) singleLine, (Int32 Begin, Int32 End) insensitive)
		{
			Newline = newline;
			Dent = dent;
			SingleLine = singleLine;
			MultiLine = insensitive;
		}
	}
	
	protected abstract SpecialTokenTypes Tokens { get; }

	protected NewlineModeLexerBase(ICharStream input, TextWriter output, TextWriter errorOutput) : base(input, output, errorOutput) {}
	
	private CommonToken CreateToken(Int32 type, String text, IToken initialIndentToken, IToken next)
	{
		return  new CommonToken(type, text)
		{
			Line = initialIndentToken.Line,
			Column = initialIndentToken.Column,
			StartIndex = initialIndentToken.StartIndex,
			StopIndex = next.StartIndex - 1
		};
	}
	
	private UInt32 dentLevel = 0;
	private readonly Stack<UInt32> newlineModeContexts = new Stack<UInt32>(new[] { 0u });
	private readonly Queue<IToken> tokenQueue = new Queue<IToken>();

	public override IToken NextToken()
	{
		if (tokenQueue.Count > 0)
			return tokenQueue.Dequeue();

		IToken next = base.NextToken();
		if (next == null)
			throw new Exception("WTF! next is null");

		if (next.Type == Tokens.SingleLine.Begin)
		{
			newlineModeContexts.Push(newlineModeContexts.Pop() + 1);
			return next;
		}
		else if (next.Type == Tokens.MultiLine.Begin)
		{
			newlineModeContexts.Push(0);
			return next;
		}
		else if (next.Type == Tokens.MultiLine.End)
		{
			if (newlineModeContexts.Count == 1)
				return next; // No multiline contexts on stack (this means we got an unpaired multiline end token)
			ProcessSingleLineTokens();
			newlineModeContexts.Pop();
			tokenQueue.Enqueue(next);
			return tokenQueue.Dequeue();
		}
		else if (next.Type == Eof)
		{
			ProcessSingleLineTokens();
			if (newlineModeContexts.Count == 1)
				EnqueueDentTokens();
			tokenQueue.Enqueue(next);
			return tokenQueue.Dequeue();
		}
		else if (next.Type == Tokens.Newline)
		{
			if (newlineModeContexts.Count > 1) // We're in multiline mode
			{
				ProcessSingleLineTokens();
				tokenQueue.Enqueue(next);
				return tokenQueue.Dequeue();
			}

			var indentationTokenCount = 0;
			IToken hold;
			var tempQueue = new Queue<IToken>();
			for (;;)
			{
				hold = base.NextToken();
				if (hold.Type == Tokens.Newline)
					indentationTokenCount = 0;
				else if (hold.Type == Tokens.Dent.Indentation)
					++indentationTokenCount;
				else
					break;
				tempQueue.Enqueue(hold);
			}

			if (indentationTokenCount == dentLevel)
			{
				ProcessSingleLineTokens();
			}
			tokenQueue.Enqueue(next);
			while (tempQueue.Count > 0)
				tokenQueue.Enqueue(tempQueue.Dequeue());

			while (indentationTokenCount != dentLevel)
			{
				if (indentationTokenCount > dentLevel)
				{
					tokenQueue.Enqueue(CreateToken(Tokens.Dent.Indent, "<Indent>", next, hold));
					++dentLevel;
				}
				else
				{
					tokenQueue.Enqueue(CreateToken(Tokens.Dent.Dedent, "<Dedent>", next, hold));
					--dentLevel;
				}
			}

			tokenQueue.Enqueue(hold);
			return tokenQueue.Dequeue();
		}
		else
		{
			return next;
		}


		// if (next.Type == SingleLineBeginToken)
		// {
		// 	newlineModeStack.Push(NewlineMode.SingleLine);
		// 	return next;
		// }

		// if (next.Type == MultiLineBeginToken)
		// {
		// 	newlineModeStack.Push(NewlineMode.Insensitive);
		// 	return next;
		// }

		// if (next.Type == MultiLineEndToken)
		// {
		// 	EnqueueSingleLineTokens();
		// 	newlineModeStack.TryPeek(out mode);
		// 	if (mode == NewlineMode.Insensitive)
		// 		newlineModeStack.Pop();
		// 	tokenQueue.Enqueue(next);
		// 	return tokenQueue.Dequeue();
		// }

		// if (next.Type == Eof)
		// {
		// 	EnqueueSingleLineTokens();
		// 	while (dentLevel > 0)
		// 	{
		// 		tokenQueue.Enqueue(CreateToken(DedentToken, "<Dedent>", next, next));
		// 		--dentLevel;
		// 	}
		// 	tokenQueue.Enqueue(next);
		// 	return tokenQueue.Dequeue();
		// }

		// if (next.Type != NewlineToken & next.Type != Eof)
		// {
		// 	return next;
		// }

		// newlineModeStack.TryPeek(out mode);
		// switch (mode)
		// {
		// 	case NewlineMode.Dented:
		// 	{
		// 		var indentationTokenCount = 0;
		// 		IToken hold;
		// 		for (;;)
		// 		{
		// 			hold = base.NextToken();
		// 			if (hold.Type == NewlineToken)
		// 			{
		// 				indentationTokenCount = 0;
		// 			}
		// 			else if (hold.Type == IndentationToken)
		// 				++indentationTokenCount;
		// 			else
		// 				break;
		// 			tokenQueue.Enqueue(hold);
		// 		}

		// 		while (indentationTokenCount != dentLevel)
		// 		{
		// 			if (indentationTokenCount > dentLevel)
		// 			{
		// 				tokenQueue.Enqueue(CreateToken(IndentToken, "<Indent>", next, hold));
		// 				++dentLevel;
		// 			}
		// 			else
		// 			{
		// 				tokenQueue.Enqueue(CreateToken(DedentToken, "<Dedent>", next, hold));
		// 				--dentLevel;
		// 			}
		// 		}
		// 		tokenQueue.Enqueue(hold);
		// 		return next;
		// 	}

		// 	case NewlineMode.SingleLine:
		// 	{
		// 		var hold = base.NextToken();
		// 		do
		// 		{
		// 			newlineModeStack.Pop();
		// 			var singleLineEnd = CreateToken(SingleLineEndToken, "<SingleLineEnd>", next, hold);
		// 			tokenQueue.Enqueue(singleLineEnd);
		// 		}
		// 		while (newlineModeStack.TryPeek(out mode) && mode == NewlineMode.SingleLine);
		// 		tokenQueue.Enqueue(hold);
		// 		return next;
		// 	}

		// 	case NewlineMode.Insensitive:
		// 		EnqueueSingleLineTokens();
		// 		tokenQueue.Enqueue(next);
		// 		return tokenQueue.Dequeue();

		// 	default:
		// 		throw new InvalidOperationException("Newline mode not supported: " + mode.ToString());
		// }

		void ProcessSingleLineTokens()
		{
			var singleLineLevel = newlineModeContexts.Pop();
			while (singleLineLevel > 0)
			{
				--singleLineLevel;
				tokenQueue.Enqueue(CreateToken(Tokens.SingleLine.End, "<SingleLineEnd>", next, next));
			}
			newlineModeContexts.Push(0u);
		}

		void EnqueueDentTokens()
		{
			while (dentLevel > 0)
			{
				--dentLevel;
				tokenQueue.Enqueue(CreateToken(Tokens.Dent.Dedent, "<Dedent>", next, next));
			}
		}
	}
}