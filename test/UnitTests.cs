using System;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
//using Antlr4.Runtime.Tree;
using Xunit;

namespace Cova.Tests
{
	public class UnitTests
	{
		[Theory]
		[InlineData("Dented.cova")]
		[InlineData("SameLine.cova")]
		[InlineData("Braced.cova")]
		public void Newline(String fileName)
		{
			using var fileStream = File.OpenRead(fileName);
			var inputStream = new AntlrInputStream(fileStream);
			var lexer = new CovaLexer(inputStream);

			Console.WriteLine(String.Join(", ", lexer.GetAllTokens().Select(x => x.Text)));
			lexer.Reset();

			var commonTokenStream = new CommonTokenStream(lexer);
			var parser = new CovaParser(commonTokenStream);
			var file = parser.file();

			// var listener = new TestListener();
			// ParseTreeWalker.Default.Walk(listener, file);

			var @namespace = Assert.Single(file.namespaceMemberDefinition());
			Assert.Equal("Foo", @namespace.namespaceDefinition().qualifiedIdentifier().GetText());

			var type = Assert.Single(@namespace.namespaceDefinition().namespaceBody().namespaceMemberDefinition());
			Assert.Equal("Bar", type.typeDefinition().identifier().GetText());

			var func = Assert.Single(type.typeDefinition().typeBody().typeMemberDefinition());
			Assert.Equal("Baz", func.functionDefinition().identifier().GetText());
		}
	}

	class TestListener : CovaParserBaseListener
	{
		public override void EnterNamespaceDefinition(CovaParser.NamespaceDefinitionContext context)
		{

		}
	}
}
