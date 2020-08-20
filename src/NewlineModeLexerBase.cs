using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Antlr4.Runtime;

public abstract class NewlineModeLexerBase : Antlr4.Runtime.Lexer
{
	private readonly SpecialTokenTypes tokens;

	protected NewlineModeLexerBase(ICharStream input, TextWriter output, TextWriter errorOutput) : base(input, output, errorOutput) =>
		tokens = GetTokenTypes();

	protected abstract SpecialTokenTypes GetTokenTypes();

	private CommonToken CreateToken(Int32 type, String name, IToken first, IToken second)
	{
		return  new CommonToken(type, "<" + name + ">")
		{
			Line = first.Line,
			Column = first.Column,
			StartIndex = first.StartIndex,
			StopIndex = second.StartIndex - 1
		};
	}

	private enum NewlineMode : Byte { Dent, MultiLine }

	private class NewlineContext
	{
		public readonly NewlineMode NewlineMode;
		public Byte ExpressionBodyLevel;
		public Byte? InitialDentLevel;
		public Byte DentLevel;

		public NewlineContext(NewlineMode newlineMode) => NewlineMode = newlineMode;
		public override String ToString() => $"Mode: `{NewlineMode}` ExprBodies: `{ExpressionBodyLevel}` Dent: {DentLevel}";
	}

	private readonly Stack<NewlineContext> newlineModeContexts = new Stack<NewlineContext>(new[] { new NewlineContext(NewlineMode.Dent) });
	private readonly Queue<IToken> tokenQueue = new Queue<IToken>();

	private NewlineContext CurrentContext => newlineModeContexts.Peek();

	public override IToken NextToken()
	{
		if (tokenQueue.Count > 0)
			return tokenQueue.Dequeue();

		IToken next = base.NextToken();
		if (next == null)
			throw new Exception("WTF! next is null");

		if (next.Type == tokens.ExpressionBody.Begin)
		{
			++CurrentContext.ExpressionBodyLevel;
			return next;
		}
		else if (next.Type == tokens.MultiLine.Begin)
		{
			newlineModeContexts.Push(new NewlineContext(NewlineMode.MultiLine));
			return next;
		}
		else if (next.Type == tokens.MultiLine.End)
		{
			// emit all synthetic tokens until we get to the top-most multi-line context, where we just emit the found token and pop

			while (newlineModeContexts.Count > 1 && CurrentContext.NewlineMode != NewlineMode.MultiLine)
			{
				EnqueueExpressionBodyEndTokens();
				EnqueueDentTokens(0, next);
				newlineModeContexts.Pop();
			}

			//if (CurrentContext.NewlineMode != NewlineMode.MultiLine)
			//{
			//	// No multiline contexts on stack (this means we got an unpaired multi-line end token, so just return it and let the parser fail)
			//	tokenQueue.Enqueue(next);
			//	return tokenQueue.Dequeue();
			//}

			tokenQueue.Enqueue(next);
			return tokenQueue.Dequeue();
		}
		else if (next.Type == Eof)
		{
			while (newlineModeContexts.Count > 0)
			{
				EnqueueExpressionBodyEndTokens();
				EnqueueDentTokens(0, next);
				newlineModeContexts.Pop();
			}

			//EnqueueSingleLineTokens();
			//if (CurrentContext.NewlineMode == NewlineMode.Dent)
			//	EnqueueDentTokens();
			tokenQueue.Enqueue(next);
			return tokenQueue.Dequeue();
		}
		else if (next.Type == tokens.Newline)
		{
			if (CurrentContext.NewlineMode == NewlineMode.MultiLine)
			{
				EnqueueExpressionBodyEndTokens();
				tokenQueue.Enqueue(next);
				return tokenQueue.Dequeue();
			}

			ProcessIndentationTokens(out Byte indentationTokenCount, out var tokensConsumed, out IToken afterNext);

			if (CurrentContext.NewlineMode == NewlineMode.MultiLine && CurrentContext.InitialDentLevel != null)
			{
				CurrentContext.InitialDentLevel = indentationTokenCount;
			}

			if (indentationTokenCount == CurrentContext.DentLevel)
			{
				EnqueueExpressionBodyEndTokens();
			}

			tokenQueue.Enqueue(next);

			foreach (var tokenConsumed in tokensConsumed)
				tokenQueue.Enqueue(tokenConsumed);

			EnqueueDentTokens(indentationTokenCount, afterNext);

			tokenQueue.Enqueue(afterNext);
			return tokenQueue.Dequeue();
		}
		else
		{
			return next;
		}

		void EnqueueExpressionBodyEndTokens()
		{
			while (CurrentContext.ExpressionBodyLevel > 0)
			{
				--CurrentContext.ExpressionBodyLevel;
				tokenQueue.Enqueue(CreateToken(tokens.ExpressionBody.End, nameof(tokens.ExpressionBody) + nameof(tokens.ExpressionBody.End), next, next));
			}
		}

		void EnqueueDentTokens(Byte newDentLevel, IToken afterNext)
		{
			while (newDentLevel != CurrentContext.DentLevel)
			{
				if (newDentLevel > CurrentContext.DentLevel)
				{
					tokenQueue.Enqueue(CreateToken(tokens.Dent.Indent, nameof(tokens.Dent.Indent), next, afterNext));
					++CurrentContext.DentLevel;
				}
				else
				{
					tokenQueue.Enqueue(CreateToken(tokens.Dent.Dedent, nameof(tokens.Dent.Dedent), next, afterNext));
					--CurrentContext.DentLevel;
				}
			}
		}

		void ProcessIndentationTokens(out Byte indentationTokenCount, out IEnumerable<IToken> tokensConsumed, out IToken next)
		{
			indentationTokenCount = 0;
			var queue = new Queue<IToken>();
			tokensConsumed = queue;
			for (;;)
			{
				next = base.NextToken();
				if (next.Type == tokens.Newline)
					indentationTokenCount = 0;
				else if (next.Type == tokens.Dent.Indentation)
					++indentationTokenCount;
				else
					break;
				queue.Enqueue(next);
			}
		}
	}

	protected struct SpecialTokenTypes
	{
		public Int32 Newline { get; }
		public (Int32 Indentation, Int32 Indent, Int32 Dedent) Dent { get; }
		public (Int32 Begin, Int32 End) ExpressionBody { get; }
		public (Int32 Begin, Int32 End) MultiLine { get; }

		public SpecialTokenTypes(
			Int32 newline,
			(Int32 Indentation, Int32 Indent, Int32 Dedent) dent,
			(Int32 Begin, Int32 End) expressionBody,
			(Int32 Begin, Int32 End) insensitive
		)
		{
			Newline = newline;
			Dent = dent;
			ExpressionBody = expressionBody;
			MultiLine = insensitive;
		}

		public static implicit operator SpecialTokenTypes((Int32 newline, (Int32 Indentation, Int32 Indent, Int32 Dedent) dent, (Int32 Begin, Int32 End) expressionBody, (Int32 Begin, Int32 End) insensitive) tokens) =>
			new SpecialTokenTypes(tokens.newline, tokens.dent, tokens.expressionBody, tokens.insensitive);
	}
}