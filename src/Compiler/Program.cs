using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Compiler.Symbols;
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

	// class AddNameQualifiersOperation : IDisposable
	// {
	// 	private readonly Int32 count;
	// 	public AddNameQualifiersOperation(IEnumerable<String> qualifiers)
	// 	{

	// 	}
	// 	public void Dispose() => end?.Invoke();
	// }

	// static class Extensions
	// {
	// 	public static IDisposable AddNameQualifiers(this List<String> nameQualifiers, IEnumerable<String> qualifiers)
	// 	=>	new Raii(() => nameQualifiers.AddRange(qualifiers), () => nameQualifiers.RemoveRange() qualifiers.Count());
	// 	public static IDisposable AddNameQualifiers(this List<String> nameQualifiers, String qualifier)
	// 	=>	new Raii(() => nameQualifiers.Add(qualifier), () => nameQualifiers.RemoveAt(nameQualifiers.Count - 1));
	// }

	public abstract class Symbol
	{
		public UInt32 Id { get; private set; }
		public abstract String Name { get; protected set;  }
	}

	public abstract class Definition : Symbol
	{
		public abstract ParserRuleContext ParserRule { get; }
	}

	//sealed class Namespace : Scope
	//{
	//	public Namespace(Scope parent) : base(parent) {}
	//}

	// class CovaParserVisitor : CovaParserBaseVisitor<Int32>
	// {
	// 	private readonly Package package;
	// 	public CovaParserVisitor(Package package) => this.package = package;

	// 	private readonly List<String> nameQualifiers = new List<String>();
	// 	public List<String> TypeNames { get; } = new List<String>();

	// 	public override Int32 VisitModuleDeclaration(CovaParser.ModuleDeclarationContext context)
	// 	{
	// 		//using var addNameQualifiers = nameQualifiers.AddNameQualifiers(context.qualifiedIdentifier().Identifier().Select(x => x.GetText()));
	// 		// var count = 0;
	// 		// foreach (var identifier in context.qualifiedIdentifier().Identifier())
	// 		// {
	// 		// 	nameQualifiers.Add(identifier.GetText());
	// 		// 	++count;
	// 		// }
	// 		var result = base.VisitModuleDeclaration(context);
	// 		// while (0 < count--)
	// 		// 	nameQualifiers.RemoveAt(nameQualifiers.Count - 1);
	// 		return result;
	// 	}

	// 	public override Int32 VisitTypeDeclaration(CovaParser.TypeDeclarationContext context)
	// 	{
	// 		using var addNameQualifiers = nameQualifiers.AddNameQualifiers(context.Identifier().GetText()));
	// 		TypeNames.Add(String.Join('.', nameQualifiers) + '.' + context.Identifier().GetText());
	// 		return base.VisitTypeDeclaration(context);
	// 	}

	// 	public override Int32 VisitFunctionDeclaration(CovaParser.FunctionDeclarationContext context)
	// 	{
	// 		return base.VisitFunctionDeclaration(context);
	// 	}

	// 	public override Int32 VisitStatement(CovaParser.StatementContext context)
	// 	{
	// 		return base.VisitStatement(context);
	// 	}
	// }
}

namespace Cova.Definitions
{
	public static class TextSourceExtensions
	{
		public static TextSourceSpan ToTextSourceSpan(this ParserRuleContext context)
		{
			var start = new TextSource(
				context.Start.TokenSource.SourceName,
				(UInt64)context.Start.StartIndex,
				(UInt64)context.Start.Column,
				(UInt64)context.Start.Line);

			var stop = new TextSource(
				context.Stop.TokenSource.SourceName,
				(UInt64)context.Stop.StartIndex,
				(UInt64)context.Stop.Column,
				(UInt64)context.Stop.Line);

			return new TextSourceSpan(start, stop);
		}
	}

	public sealed record TextSource(String Path, UInt64 Offset, UInt64 Line, UInt64 Column);
	public sealed record TextSourceSpan(TextSource Start, TextSource Stop);

	public interface IDefinition
	{
		UInt64 Id { get; }
		String Name { get; }
		TextSourceSpan Location { get; }
		IDefinition? Parent { get; }
	}

	public abstract class Definition
	{
		public UInt64 Id { get; private set; }
		public String Name { get; set; } = null!;
	}

	public abstract class SourceTextDefinition : Definition, IDefinition
	{
		public TextSourceSpan Location { get; set; } = null!;
		public IDefinition? Parent { get; set; }
	}

	public sealed class Package : Definition, IHasModules
	{
		public HashSet<Module> Modules { get; } = new();
	}

	public class Module : Definition, IHasTypes, IHasFunctions
	{
		public HashSet<Type> Types { get; } = new();
		public HashSet<Function> Functions { get; } = new();
	}

	public class Type : SourceTextDefinition, IHasTypes, IHasFunctions
	{
		public Module Module { get; set; } = null!;
		public HashSet<Type> Types { get; } = new();
		public HashSet<Function> Functions { get; } = new();
	}

	public class Function : SourceTextDefinition, IHasStatements
	{
		public Module Module { get; set; } = null!;
		public HashSet<Statement> Statements { get; } = new();
	}

	public class Statement : SourceTextDefinition
	{
		public new Function Parent { get; private set; } = null!;
	}

	public interface IHasModules { public HashSet<Module> Modules { get; } }
	public interface IHasTypes { public HashSet<Type> Types { get; } }
	public interface IHasFunctions { public HashSet<Function> Functions { get; } }
	public interface IHasStatements { public HashSet<Statement> Statements { get; } }
	//public interface IHasParent { public IDefinition Parent { get; } }
	//public interface IHasParent<TParent> : IHasParent where TParent : IDefinition
	//{
	//	public new TParent Parent { get; }
	//	IDefinition IHasParent.Parent => Parent;
	//}
}