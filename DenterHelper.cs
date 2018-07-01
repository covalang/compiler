using System;
using System.Collections.Generic;
using Antlr4.Runtime;

public abstract class DenterOptions
{
	/**
	 * Don't do any special handling for EOFs; they'll just be passed through normally. That is, we won't unwind indents
	 * or add an extra NL.
	 *
	 * This is useful when the lexer will be used to parse rules that are within a line, such as expressions. One use
	 * case for that might be unit tests that want to exercise these sort of "line fragments".
	 */
	public abstract void IgnoreEOF();

	protected readonly DenterHelper DenterHelper;
	protected DenterOptions(DenterHelper denterHelper) => DenterHelper = denterHelper;
}

public class DenterHelper
{
	private readonly Queue<IToken> dentsBuffer = new Queue<IToken>();
	private readonly Stack<int> indentations = new Stack<int>();
	private readonly int nlToken;
	private readonly int indentToken;
	private readonly int dedentToken;
	private Boolean reachedEof;
	private EofHandler eofHandler;

	public DenterHelper(int nlToken, int indentToken, int dedentToken, Func<DenterHelper, IToken> pullTokenFunc)
	{
		this.nlToken = nlToken;
		this.indentToken = indentToken;
		this.dedentToken = dedentToken;
		eofHandler = new StandardEofHandler(this);
		this.pullTokenFunc = pullTokenFunc;
	}



	abstract class EofHandler
	{
		protected readonly DenterHelper DenterHelper;
		protected EofHandler(DenterHelper denterHelper) => DenterHelper = denterHelper;

		public abstract IToken Apply(IToken t);
	}

	class EofHandlerWithDelegate : EofHandler
	{
		public EofHandlerWithDelegate(DenterHelper denterHelper) : base(denterHelper) { }
		public override IToken Apply(IToken t) => ApplyFunc(t);
		public Func<IToken, IToken> ApplyFunc { get; set; }
	}

	sealed class StandardEofHandler : EofHandler
	{
		public override IToken Apply(IToken t)
		{
			IToken r;
			// when we reach EOF, unwind all indentations. If there aren't any, insert a NL. This lets the grammar treat
			// un-indented expressions as just being NL-terminated, rather than NL|EOF.
			if (DenterHelper.indentations.Count == 0)
			{
				r = DenterHelper.createToken(DenterHelper.nlToken, t);
				DenterHelper.dentsBuffer.Enqueue(t);
			}
			else
			{
				r = DenterHelper.unwindTo(0, t);
				DenterHelper.dentsBuffer.Enqueue(t);
			}
			DenterHelper.reachedEof = true;
			return r;
		}

		public StandardEofHandler(DenterHelper denterHelper) : base(denterHelper) { }
	}



	public IToken nextToken()
	{
		initIfFirstRun();
		IToken t = dentsBuffer.Count == 0
		  ? pullToken()
		  : dentsBuffer.Dequeue();
		if (reachedEof)
		{
			return t;
		}
		IToken r;
		if (t.Type == nlToken)
		{
			r = handleNewlineToken(t);
		}
		else if (t.Type == TokenConstants.EOF)
		{
			r = eofHandler.Apply(t);

		}
		else
		{
			r = t;
		}
		return r;
	}

	public DenterOptions getOptions()
	{
		return new DenterOptionsImpl(this);
	}

	protected IToken pullToken() => pullTokenFunc(this);
	private readonly Func<DenterHelper, IToken> pullTokenFunc;

	private void initIfFirstRun()
	{
		if (indentations.Count == 0)
		{
			indentations.Push(0);
			// First invocation. Look for the first non-NL. Enqueue it, and possibly an indentation if that non-NL
			// token doesn't start at char 0.
			IToken firstRealToken;
			do
			{
				firstRealToken = pullToken();
			}
			while (firstRealToken.Type == nlToken);

			if (firstRealToken.Column > 0)
			{
				indentations.Push(firstRealToken.Column);
				dentsBuffer.Enqueue(createToken(indentToken, firstRealToken));
			}
			dentsBuffer.Enqueue(firstRealToken);
		}
	}

	private IToken handleNewlineToken(IToken t)
	{
		// fast-forward to the next non-NL
		IToken nextNext = pullToken();
		while (nextNext.Type == nlToken)
		{
			t = nextNext;
			nextNext = pullToken();
		}
		if (nextNext.Type == TokenConstants.EOF)
		{
			return eofHandler.Apply(nextNext);
		}
		// nextNext is now a non-NL token; we'll queue it up after any possible dents

		String nlText = t.Text;
		int indent = nlText.Length - 1; // every NL has one \n char, so shorten the length to account for it
		if (indent > 0 && nlText[0] == '\r')
		{
			--indent; // If the NL also has a \r char, we should account for that as well
		}
		int prevIndent = indentations.Peek();
		IToken r;
		if (indent == prevIndent)
		{
			r = t; // just a newline
		}
		else if (indent > prevIndent)
		{
			r = createToken(indentToken, t);
			indentations.Push(indent);
		}
		else
		{
			r = unwindTo(indent, t);
		}
		dentsBuffer.Enqueue(nextNext);
		return r;
	}

	private IToken createToken(int tokenType, IToken copyFrom)
	{
		String tokenTypeStr;
		if (tokenType == nlToken)
		{
			tokenTypeStr = "newline";
		}
		else if (tokenType == indentToken)
		{
			tokenTypeStr = "indent";
		}
		else if (tokenType == dedentToken)
		{
			tokenTypeStr = "dedent";
		}
		else
		{
			tokenTypeStr = null;
		}
		CommonToken r = new InjectedToken(copyFrom, tokenTypeStr);
		r.Type = tokenType;
		return r;
	}

	/**
	 * Returns a DEDENT token, and also queues up additional DEDENTS as necessary.
	 * @param targetIndent the "size" of the indentation (number of spaces) by the end
	 * @param copyFrom the triggering token
	 * @return a DEDENT token
	 */
	private IToken unwindTo(int targetIndent, IToken copyFrom)
	{
		if (dentsBuffer.Count != 0)
			throw new Exception();
		dentsBuffer.Enqueue(createToken(nlToken, copyFrom));
		// To make things easier, we'll queue up ALL of the dedents, and then pop off the first one.
		// For example, here's how some text is analyzed:
		//
		//  Text          :  Indentation  :  Action         : Indents Deque
		//  [ baseline ]  :  0            :  nothing        : [0]
		//  [   foo    ]  :  2            :  INDENT         : [0, 2]
		//  [    bar   ]  :  3            :  INDENT         : [0, 2, 3]
		//  [ baz      ]  :  0            :  DEDENT x2      : [0]

		while (true)
		{
			int prevIndent = indentations.Pop();
			if (prevIndent == targetIndent)
			{
				break;
			}
			if (targetIndent > prevIndent)
			{
				// "weird" condition above
				indentations.Push(prevIndent); // restore previous indentation, since we've indented from it
				dentsBuffer.Enqueue(createToken(indentToken, copyFrom));
				break;
			}
			dentsBuffer.Enqueue(createToken(dedentToken, copyFrom));
		}
		indentations.Push(targetIndent);
		return dentsBuffer.Dequeue();
	}

	private class DenterOptionsImpl : DenterOptions
	{

		public override void IgnoreEOF()
		{
			DenterHelper.eofHandler = new EofHandlerWithDelegate(DenterHelper)
			{
				ApplyFunc = t =>
				{
					DenterHelper.reachedEof = true;
					return t;
				}
			};
		}

		public DenterOptionsImpl(DenterHelper denterHelper) : base(denterHelper) { }
	}

	class InjectedToken : CommonToken
	{

		private String type;

		public InjectedToken(IToken oldToken, String type) : base(oldToken)
		{
			this.type = type;
		}

		public override String Text {
			get {
				if (type != null)
					Text = type;
				return base.Text;
			}
		}
	}

	public interface Builder0
	{
		Builder1 nl(int nl);
	}

	public interface Builder1
	{
		Builder2 indent(int indent);
	}

	public interface Builder2
	{
		Builder3 dedent(int dedent);
	}

	public interface Builder3
	{
		DenterHelper pullToken(TokenPuller puller);
	}

	public interface TokenPuller
	{
		IToken pullToken();
	}

	public static Builder0 builder()
	{
		return new BuilderImpl();
	}

	private class BuilderImpl : Builder0, Builder1, Builder2, Builder3
	{

		private int nl;
		private int indent;
		private int dedent;

		Builder1 Builder0.nl(int nl)
		{
			this.nl = nl;
			return this;
		}

		Builder2 Builder1.indent(int indent)
		{
			this.indent = indent;
			return this;
		}

		Builder3 Builder2.dedent(int dedent)
		{
			this.dedent = dedent;
			return this;
		}

		DenterHelper Builder3.pullToken(TokenPuller puller)
		{
			TokenPuller p = puller;
			return new DenterHelper(nl, indent, dedent, x => p.pullToken());
		}
	}
}