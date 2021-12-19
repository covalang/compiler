using System;
using Antlr4.Runtime;
using Cova.Compiler.Parser;
using Xunit;

namespace Compiler.Parser.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var input = @"use namespace Foo;
namespace Bar{
    type Baz {}";
            var lexer = new CovaLexerExtended(new CodePointCharStream(input));
            var tokens = lexer.GetAllTokens();

        }
    }
}