﻿using System;
using System.IO;
using Antlr4.Runtime;

namespace Cova.Compiler.Parser
{
    public class CovaParserExtended : Grammar.CovaParser
    {
        public CovaParserExtended(ITokenStream input) : base(input) { }
        public CovaParserExtended(ITokenStream input, TextWriter output, TextWriter errorOutput) : base(input, output, errorOutput) { }

        public CovaParserExtended(String input, String name) : this(
            new CommonTokenStream(
                new CovaLexerExtended(
                    new CodePointCharStream(input) { name = name })
                )
            )
        { }
    }
}