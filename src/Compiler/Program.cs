using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
//using Compiler.Symbols;
using Cova.Compiler.Parser;
using Cova.Compiler.Parser.Grammar;
using Cova.Scopes;
using Cova.Symbols;
using LLVMSharp.Interop;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[assembly: CLSCompliant(false)]

namespace Cova
{
	class Program
	{
		static void Main(String[] args)
		{
			var filename = "Test.cova";
			var filePath = File.Exists(filename) ? filename : "../../../" + filename;
			var fileContents = File.ReadAllText(filePath);

			var parser = new CovaParserExtended(fileContents, filename);
			parser.Interpreter.PredictionMode = PredictionMode.SLL;
			var file = parser.file();

			var rootSymbol = InterfaceImplementor.CreateAndInitialize<IModule>();
			rootSymbol.Name = "Module";
			rootSymbol.DefinitionSource = file.ToTextSourceSpan();

			var symReg = new SymbolRegistrationVisitor(rootSymbol);
			symReg.Visit(file);
		}

		static void Main2(String[] args)
		{
			//var result =
			//	CommandLine
			//		.Parser
			//		.Default
			//		.ParseArguments(args, typeof(Commands).GetNestedTypes())
			//		.MapResult(
			//			(Commands.Serve serve) => serve.Debug,
			//			errors => Console.WriteLine(String.Join(Environment.NewLine, errors))
			//		);

			//return Host.CreateDefaultBuilder().RunConsoleAsync();

			var filename = "Test.cova";
			var filePath = File.Exists(filename) ? filename : "../../../" + filename;
			var fileContents = File.ReadAllText(filePath);
			var inputStream = new CodePointCharStream(fileContents) { name = filePath };
			var lexer = new CovaLexer(inputStream);

			// IToken token;
			// Int32 indents = -1;
			// UInt32 line = 1;
			// //Console.Write($"{line,-3}");
			// while ((token = lexer.NextToken()).Type != Lexer.Eof)
			// {
			// 	Console.Write(token.Text);
			// 	// var tokenTypeName = lexer.GetTokenTypeName(token.Type);
			// 	// if (tokenTypeName == "Newline")
			// 	// {
			// 	// 	Console.WriteLine();
			// 	// 	Console.Write($"{++line,-3}");
			// 	// }
			// 	// else if (tokenTypeName == "Indent")
			// 	// {
			// 	// 	indents++;
			// 	// 	Console.Write(new String('\t', indents));
			// 	// }
			// 	// else if (tokenTypeName == "Dedent")
			// 	// {
			// 	// 	indents--;
			// 	// 	Console.Write(new String('\t', indents));
			// 	// }
			// 	// else if (tokenTypeName == "Dent" | tokenTypeName == "Tab")
			// 	// {}
			// 	// else
			// 	// 	Console.Write(tokenTypeName + " ");
			// }
			// lexer.Reset();
			// fileStream.Position = 0;
			// return;

			var commonTokenStream = new CommonTokenStream(lexer);
			var parser = new CovaParser(commonTokenStream);// { BuildParseTree = false };
			parser.Interpreter.PredictionMode = PredictionMode.SLL;
			//var package = new Package { Name = "Cova" };

			//using var llvmInitializer = new LLVMInitializer();
			//using var module = LLVMModuleRef.CreateWithName("NativeBinary");
			//var builder = module.Context.CreateBuilder();

			////using var connection = new SqliteConnection("Data Source=:memory:");
			//using var connection = new SqliteConnection("Data Source=Sharable;Mode=Memory;Cache=Shared");
			//connection.Open();
			//using var context = new Context(connection);
			//context.Database.EnsureDeleted();
			//context.Database.EnsureCreated();
			//context.Database.ExecuteSqlRaw("pragma auto_vacuum = full;");
			//var funcs = context.Functions.ToList();

			//var rootScope = new RootScope();
			//var fileScope = new FileScope(filename, rootScope);
			var rootSymbol = InterfaceImplementor.CreateAndInitialize<IModule>();
			rootSymbol.Name = "Module";
			var symReg = new SymbolRegistrationVisitor(rootSymbol);
			symReg.Visit(parser.file());
			//var symbolReg = new SymbolRegistrationListener(rootSymbol);
			//ParseTreeWalker.Default.Walk(symbolReg, parser.file());
			var symbolRes = new SymbolResolutionListener(rootSymbol);
		}
	}

	sealed class LLVMInitializer : IDisposable
	{
		public LLVMInitializer()
		{
			LLVM.InitializeNativeTarget();
			LLVM.InitializeAllAsmParsers();
			LLVM.InitializeAllAsmPrinters();
		}

		public void Dispose() => LLVM.Shutdown();
	}

	class Raii : IDisposable
	{
		private readonly Action end;
		public Raii(Action start, Action end)
		{
			start();
			this.end = end;
		}
		public void Dispose() => end?.Invoke();
	}
}