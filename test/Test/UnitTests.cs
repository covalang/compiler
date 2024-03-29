﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
using Cova.Compiler.Parser.Grammar;
//using Antlr4.Runtime.Tree;
using Xunit;

namespace Cova.Tests
{

	public class UnitTests
	{
		[Theory]
		[MemberData(nameof(Data))]
		public void NewlinesFile(String source, String expected)
		{
			var inputStream = new AntlrInputStream(source);
			var lexer = new CovaLexer(inputStream);
			string joinedTokens = String.Join(String.Empty, lexer.GetAllTokens().Select(x => x.Text));
			Assert.Equal(expected, joinedTokens);
		}

		public static IEnumerable<String[]> Data =>
			File.ReadAllText("Newlines.cova").Split("\n\n###\n\n").Select(x => x.Split("\n===\n"));

		//		[Fact]
		//		public void DentedOnly()
		//		{
		//			var source = @"
		//namespace Foo
		//	type Bar
		//		func Baz";
		//			var expected = @"
		//►namespace Foo
		//	►type Bar
		//		►func Baz◄◄◄";
		//			AssertSourceEqualsTokenized(expected, source);
		//		}

		//		[Fact]
		//		public void InsensitiveOnlyMultiLine()
		//		{
		//			var source = @"namespace Foo {
		//	type Bar {
		//		func Baz {}
		//	}
		//}";
		//			AssertSourceEqualsTokenized(source, source);
		//		}

		//[Fact]
		//public void InsensitiveOnlySingleLine()
		//{
		//	var source = "namespace Foo { type Bar { func Baz {} } }";
		//	AssertSourceEqualsTokenized(source, source);
		//}

		//[Fact]
		//public void SingleLineOnly()
		//{
		//	var source = "namespace Foo -> type Bar -> func Baz -> a = b";
		//	var expected = "namespace Foo -> type Bar -> func Baz -> a = b<ExpressionBodyEnd><ExpressionBodyEnd><ExpressionBodyEnd>";
		//	AssertSourceEqualsTokenized(expected, source);
		//}

		//[Fact]
		//public void SingleLineInsideInsensitiveOnly()
		//{
		//	var source = "namespace Foo { type Bar -> func Baz {} }";
		//	var expected = "namespace Foo { type Bar -> func Baz {} <ExpressionBodyEnd>}";
		//	AssertSourceEqualsTokenized(expected, source);
		//}

		//[Fact]
		//public void DentedInsideTwoSingleLines()
		//{
		//	var source = "namespace Foo -> type Bar -> func Baz\n\ta = b";
		//	var expected = "namespace Foo -> type Bar -> func Baz\n\t<Indent>a = b<Dedent><ExpressionBodyEnd><ExpressionBodyEnd>";
		//	AssertSourceEqualsTokenized(expected, source);
		//}

		//[Fact]
		//public void SingleLineInsideDentedInsideSingleLine()
		//{
		//	var source = "namespace Foo -> type Bar\n\tfunc Baz -> a = b";
		//	var expected = "namespace Foo -> type Bar\n\t<Indent>func Baz -> a = b<Dedent><ExpressionBodyEnd><ExpressionBodyEnd>";
		//	AssertSourceEqualsTokenized(expected, source);
		//}

		private void AssertSourceEqualsTokenized(String tokenized, String source)
		{
			var inputStream = new AntlrInputStream(source);
			var lexer = new CovaLexer(inputStream);
			string joinedTokens = String.Join(String.Empty, lexer.GetAllTokens().Select(x => x.Text));
			Assert.Equal(tokenized, joinedTokens);
		}

		//[Theory]
		//[InlineData("Dented.cova")]
		//[InlineData("SameLine.cova")]
		//[InlineData("Braced.cova")]
		//public void Newline(String fileName)
		//{
		//	using var fileStream = File.OpenRead(fileName);
		//	var inputStream = new AntlrInputStream(fileStream);
		//	var lexer = new CovaLexer(inputStream);

		//	Console.WriteLine(String.Join(", ", lexer.GetAllTokens().Select(x => x.Text)));
		//	lexer.Reset();

		//	var commonTokenStream = new CommonTokenStream(lexer);
		//	var parser = new CovaParser(commonTokenStream);
		//	var file = parser.file();

		//	// var listener = new TestListener();
		//	// ParseTreeWalker.Default.Walk(listener, file);

		//	var @namespace = Assert.Single(file.namespaceMemberDefinition());
		//	Assert.Equal("Foo", @namespace.namespaceDefinition().qualifiedIdentifier().GetText());

		//	var type = Assert.Single(@namespace.namespaceDefinition().namespaceBody().namespaceMemberDefinition());
		//	Assert.Equal("Bar", type.typeDefinition().identifier().GetText());

		//	var func = Assert.Single(type.typeDefinition().typeBody().typeMemberDefinition());
		//	Assert.Equal("Baz", func.functionDefinition().identifier().GetText());
		//}
	}

	class TestListener : CovaParserBaseListener
	{
		public override void EnterNamespaceDefinition(CovaParser.NamespaceDefinitionContext context)
		{

		}
	}
}
