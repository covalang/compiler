using System;
using System.Collections.Generic;
using System.Numerics;
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
			var what = new GyooVisitor<Int32>();
            what.Visit(parser.program());
        }
    }

    class GyooVisitor<TResult> : GyooBaseVisitor<TResult> {
        private readonly Dictionary<String, BigInteger> variables = new Dictionary<String, BigInteger>();

        public override TResult VisitAdd(GyooParser.AddContext context) {
            var sourceId = context.GetChild(1).GetText();
            var destinationId = context.GetChild(3).GetText();
            if (BigInteger.TryParse(sourceId, out var number))
                variables[destinationId] += number;
            else
                variables[destinationId] += variables[sourceId];
            return default;
        }
        public override TResult VisitPrint(GyooParser.PrintContext context) {
            var id = context.GetChild(1).GetText();
            if (BigInteger.TryParse(id, out var number))
                Console.WriteLine(number);
            else
                Console.WriteLine(variables[id]);
            return default;
        }
        public override TResult VisitAssign(GyooParser.AssignContext context) {
            var id = context.GetChild(1).GetText();
            var number = BigInteger.Parse(context.GetChild(3).GetText());
            variables.Add(id, number);
            return default;
        }
    }
}
