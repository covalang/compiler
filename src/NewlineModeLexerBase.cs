using System;
using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;

public abstract class NewlineModeLexerBase : Antlr4.Runtime.Lexer
{
	protected abstract Int32 NewlineToken { get; }
	protected abstract Int32 IndentationToken { get; }
	protected abstract Int32 IndentToken { get; }
	protected abstract Int32 DedentToken { get; }
	protected abstract Int32 SingleLineBeginToken { get; }
	protected abstract Int32 SingleLineEndToken { get; }
	protected abstract Int32 MultiLineBeginToken { get; }
	protected abstract Int32 MultiLineEndToken { get; }

	protected NewlineModeLexerBase(ICharStream input) : base(input) {}
	public  NewlineModeLexerBase(ICharStream input, TextWriter output, TextWriter errorOutput) : base(input, output, errorOutput) {}
	
	private CommonToken createToken(Int32 type, String text, IToken initialIndentToken, IToken next)
	{
		return  new CommonToken(type, text)
		{
			Line = initialIndentToken.Line,
			Column = initialIndentToken.Column,
			StartIndex = initialIndentToken.StartIndex,
			StopIndex = next.StartIndex - 1
		};
	}
	
	public enum NewlineMode { Dented, SingleLine, Insensitive }

	private Int32 dentLevel = 0;
	private readonly Queue<IToken> tokenQueue = new Queue<IToken>();
	private readonly Stack<NewlineMode> newlineModeStack = new Stack<NewlineMode>();

	public override IToken NextToken()
	{
		
		if (tokenQueue.Count > 0)
			return tokenQueue.Dequeue();

		IToken next = base.NextToken();
		if (next == null)
			throw new Exception("WTF! next is null");

		if (next.Type == SingleLineBeginToken)
		{
			newlineModeStack.Push(NewlineMode.SingleLine);
			return next;
		}

		if (next.Type == MultiLineBeginToken)
		{
			newlineModeStack.Push(NewlineMode.Insensitive);
			return next;
		}

		if (next.Type == MultiLineEndToken)
		{
			newlineModeStack.TryPeek(out var topMode);
			if (topMode == NewlineMode.Insensitive)
				newlineModeStack.Pop();
			return next;
		}

		if (next.Type == Eof)
		{
			while (dentLevel > 0)
			{
				tokenQueue.Enqueue(createToken(DedentToken, "Dedent", next, next));
				--dentLevel;
			}
			tokenQueue.Enqueue(next);
			return tokenQueue.Dequeue();
		}

		if (next.Type != NewlineToken)
		{
			return next;
		}

		newlineModeStack.TryPeek(out var mode);
		switch (mode)
		{
			case NewlineMode.Dented:
			{
				var indentationTokenCount = 0;
				IToken hold;
				for (;;)
				{
					hold = base.NextToken();
					if (hold.Type == NewlineToken)
					{
						indentationTokenCount = 0;
					}
					else if (hold.Type == IndentationToken)
						++indentationTokenCount;
					else
						break;
					tokenQueue.Enqueue(hold);
				}

				while (indentationTokenCount != dentLevel)
				{
					if (indentationTokenCount > dentLevel)
					{
						tokenQueue.Enqueue(createToken(IndentToken, "Indent", next, hold));
						++dentLevel;
					}
					else
					{
						tokenQueue.Enqueue(createToken(DedentToken, "Dedent", next, hold));
						--dentLevel;
					}
				}
				tokenQueue.Enqueue(hold);
				return next;
			}

			case NewlineMode.SingleLine:
			{
				var hold = base.NextToken();
				do
				{
					newlineModeStack.Pop();
					var singleLineEnd = createToken(SingleLineEndToken, "SingleLineEnd", next, hold);
					tokenQueue.Enqueue(singleLineEnd);
				}
				while (newlineModeStack.TryPeek(out mode) && mode == NewlineMode.SingleLine);
				tokenQueue.Enqueue(hold);
				return next;
			}

			case NewlineMode.Insensitive:
				return next;

			default:
				throw new InvalidOperationException("Newline mode not supported: " + mode.ToString());
		}
	}
}