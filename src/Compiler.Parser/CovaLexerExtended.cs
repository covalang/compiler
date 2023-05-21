using System;
using Antlr4.Runtime;
using System.IO;

namespace Cova.Compiler.Parser
{
    public sealed class CovaLexerExtended : Grammar.CovaLexer
    {
        public CovaLexerExtended(ICharStream input) : base(input) { }
        public CovaLexerExtended(ICharStream input, TextWriter output, TextWriter errorOutput) : base(input, output, errorOutput) { }
        public CovaLexerExtended(String input, String name) : this(new CodePointCharStream(input) { name = name }) { }
        
        public override IToken NextToken() => linearHelper.NextToken(() => dentHelper.NextToken(base.NextToken));

        private readonly DentHelper dentHelper = new DentHelper(
            newlineToken: Newline,
            indentationToken: Tab,
            indentToken: (Indent, "►"),
            dentToken: (Dent, "■"),
            dedentToken: (Dedent, "◄"),
            otherBlockTerminals: new [] {
                (LeftBrace, RightBrace)
            }
        );

        private readonly LinearHelper linearHelper = new LinearHelper(
            linearBodyBegin: Arrow,
            linearBodyEnd: (LinearBodyEnd, "♦"),
            defaultBodyTerminals: (Indent, Dent, Dedent),
            otherBodyTerminals: new [] {
                (LeftBrace, SemiColon, RightBrace),
                (LeftParenthesis, Comma, RightParenthesis),
                (LeftBracket, Comma, RightBracket)
            }
        );
    }
}
