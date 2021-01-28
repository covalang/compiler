using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using LLVMSharp.Interop;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static CovaParser;

[assembly: CLSCompliant(false)]

namespace Cova
{
	class Program
	{

		static Task Main(String[] args)
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

			return Host.CreateDefaultBuilder().RunConsoleAsync();

			var filename = "Test.cova";
			var filePath = File.Exists(filename) ? filename : "../../../" + filename;
			using var fileStream = File.OpenRead(filePath);
			var inputStream = CharStreams.fromStream(fileStream); //new AntlrInputStream(fileStream);
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
			var parser = new CovaParser(commonTokenStream);
			var package = new Package("cova");

			//using var llvmInitializer = new LLVMInitializer();
			//using var module = LLVMModuleRef.CreateWithName("NativeBinary");
			//var builder = module.Context.CreateBuilder();

			//using var connection = new SqliteConnection("Data Source=:memory:");
			using var connection = new SqliteConnection("Data Source=Sharable;Mode=Memory;Cache=Shared");
			connection.Open();
			using var context = new Context(connection);
			context.Database.EnsureDeleted();
			context.Database.EnsureCreated();
			context.Database.ExecuteSqlRaw("pragma auto_vacuum = full;");
			var funcs = context.Functions.ToList();

			var listener = new CovaListener(filename);
			ParseTreeWalker.Default.Walk(listener, parser.file());
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

	class CovaListener : CovaParserBaseListener, IRootScope
	{
		//private readonly Scope fileScope;
		readonly String filename;
		public CovaListener(String filename) => this.filename = filename;

		HashSet<IScope> Children { get; } = new HashSet<IScope>();
		HashSet<IScope> Imported { get; } = new HashSet<IScope>();
		IReadOnlySet<IScope> IScopeBase.Children => Children;
		IReadOnlySet<IScope> IScopeBase.Imported => Imported;


		private readonly List<ParserRuleContext> qualifierStack = new List<ParserRuleContext>();

		private String CurrentQualifier => String.Join('.', CurrentQualifiers.Select(x => x.GetText()));

		private IEnumerable<IdentifierContext> CurrentQualifiers => qualifierStack
			.SelectMany(
				x => x switch
				{
					QualifiedIdentifierContext qic => qic.identifier(),
					IdentifierContext ic => new[] { ic },
					_ => throw new InvalidOperationException()
				}
			);

		public override void EnterFile([NotNull] FileContext context)
		{
			context.Name = filename;
			Children.Add(context);
		}

		public override void EnterNamespaceDefinition(NamespaceDefinitionContext context)
		{
			qualifierStack.AddRange(context.qualifiedIdentifier().identifier());
		}

		public override void ExitNamespaceDefinition(NamespaceDefinitionContext context)
		{
			qualifierStack.RemoveAt(qualifierStack.Count - 1);
		}

		public override void EnterTypeDefinition(TypeDefinitionContext context)
		{
			qualifierStack.Add(context.identifier());
		}

		public override void ExitTypeDefinition(TypeDefinitionContext context)
		{
			qualifierStack.RemoveAt(qualifierStack.Count - 1);
		}

		public override void EnterFunctionDefinition(FunctionDefinitionContext context)
		{
			qualifierStack.Add(context.identifier());
			//Console.WriteLine(CurrentQualifier);

		}

		public override void ExitFunctionDefinition(FunctionDefinitionContext context)
		{
			qualifierStack.RemoveAt(qualifierStack.Count - 1);
		}

		//public override void EnterLocalDefinition(LocalDefinitionContext context)
		//{
		//}

		//public override void EnterSequenceExpression(SequenceExpressionContext context)
		//{
		//	var expressions = context.expression();
		//	var lower = expressions[0];
		//	var upper = expressions[1];
		//	var interval = expressions[3];
		//}
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

	public class Context : DbContext
	{
		readonly DbConnection dbConnection;
		
		public Context(DbConnection dbConnection) => this.dbConnection = dbConnection;

		public DbSet<Package> Packages => Set<Package>();
		public DbSet<Module> Modules => Set<Module>();
		public DbSet<Type> Types => Set<Type>();
		public DbSet<Function> Functions => Set<Function>();
		public DbSet<Statement> Statements => Set<Statement>();
		protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite(dbConnection);//("DataSource=file:memdb1?mode=memory&cache=shared");//"Data Source=Graph.db;Cache=Shared");
		protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.MapTablePerType(this);
	}

	public static class ModelBuilderExtensions
	{
		public static void MapTablePerType(this ModelBuilder modelBuilder, DbContext context)
		{
			var entitySets = context
				.GetType()
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
				.Select(x => (entityType: x.PropertyType.GetGenericArguments().Single(), name: x.Name));
			foreach (var (entityType, name) in entitySets)
				modelBuilder.Entity(entityType).ToTable(name);
		}
	}

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

	interface IDefinition
	{
		public Int64 Id { get; }
		public String Name { get; }
	}

	public sealed class Package : IDefinition
	{
		private Package() { }
		public Package(String name) => Name = name;

		public Int64 Id { get; private set; }
		public String Name { get; }
		public HashSet<Module> Modules { get; } = new HashSet<Module>();
	}

	public class Module : IDefinition
	{
		private Module() { }
		public Module(String name) => Name = name;

		public Int64 Id { get; private set; }
		public String Name { get; private set; } = null!;
		public Package? Parent { get; set; }
		public HashSet<Type> Types { get; } = new HashSet<Type>();
		public HashSet<Function> Functions { get; } = new HashSet<Function>();
	}

	public class Type : IDefinition
	{
		private Type() { }
		public Type(String name, Module module) => (Name, Module) = (name, module);

		public Int64 Id { get; private set; }
		public String Name { get; private set; } = null!;
		public Module Module { get; private set; } = null!;
		public Type? Parent { get; set; }
		public HashSet<Type> Types { get; } = new HashSet<Type>();
		public HashSet<Function> Functions { get; } = new HashSet<Function>();
	}

	public class Function : IDefinition
	{
		private Function() { }
		public Function(String name, Module module) => (Name, Module) = (name, module);

		public Int64 Id { get; private set; }
		public String Name { get; private set; } = null!;
		public Module Module { get; private set; } = null!;
		public Type? Parent { get; set; }
		public HashSet<Statement> Statements { get; } = new HashSet<Statement>();
	}

	public class Statement : IDefinition
	{
		private Statement() { }
		public Statement(String name, Function parent) => (Name, Parent) = (name, parent);

		public Int64 Id { get; private set; }
		public String Name { get; private set; } = null!;
		public Function Parent { get; private set; } = null!;
	}

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
