using System;
using Antlr4.Runtime;

namespace Cova
{
    class Program
    {
        static void Main(String[] args)
        {
            var input = 
@"begin
    let a be 5
    let b be 10
    add 3 to b
    add b to a
    add a to b
    print b
    print 3
end";

            var stream = CharStreams.fromstring(input);
			var lexer = new GyooLexer(stream);
			var tokens = new CommonTokenStream(lexer);
			var parser = new GyooParser(tokens);
			var what = new GyooVisitor();
            what.Visit(parser.program());
        }
    }

    class GyooVisitor : GyooBaseVisitor<Int32> {
        public override Int32 VisitAssign(GyooParser.AssignContext context) {
            return 42;
        }
    }
}
