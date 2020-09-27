using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[assembly: System.CLSCompliant(false)]

namespace Cova
{
	class Program
	{
		static void Main()
		{
			var filename = "Main3.cova";
			var filPath = File.Exists(filename) ? filename : "../../../" + filename;
			using var fileStream = File.OpenRead(filPath);
			var inputStream = new AntlrInputStream(fileStream);
			var lexer = new CovaLexer(inputStream);

			IToken token;
			while ((token = lexer.NextToken()).Type != Lexer.Eof)
				Console.Write(token.Text);
			lexer.Reset();
			return;

			var commonTokenStream = new CommonTokenStream(lexer);
			var parser = new CovaParser(commonTokenStream);
			var package = new Package("cova");

			var listener = new CovaListener();
			ParseTreeWalker.Default.Walk(listener, parser.file());
		}
	}

	class CovaListener : CovaParserBaseListener
	{
		private readonly List<ParserRuleContext> qualifierStack = new List<ParserRuleContext>();
		private String CurrentQualifier => String.Join('.', qualifierStack.SelectMany(x => x is CovaParser.QualifiedIdentifierContext qic ? qic.identifier() : x is CovaParser.IdentifierContext ic ? new[] { ic } : throw new InvalidOperationException()).Select(x => x.GetText()));
		
		public override void EnterNamespaceDefinition([NotNull] CovaParser.NamespaceDefinitionContext context)
		{
			qualifierStack.Add(context.qualifiedIdentifier());
			Console.WriteLine(CurrentQualifier);
		}
		
		public override void ExitNamespaceDefinition([NotNull] CovaParser.NamespaceDefinitionContext context)
		{
			qualifierStack.RemoveAt(qualifierStack.Count - 1);
		}
		
		public override void EnterTypeDefinition([NotNull] CovaParser.TypeDefinitionContext context)
		{
			qualifierStack.Add(context.identifier());
			Console.WriteLine(CurrentQualifier);


		}
		
		public override void ExitTypeDefinition([NotNull] CovaParser.TypeDefinitionContext context)
		{
			qualifierStack.RemoveAt(qualifierStack.Count - 1);
		}
		
		//public override void EnterLocalDefinition([NotNull] CovaParser.LocalDefinitionContext context)
		//{
		//}

		//public override void EnterSequenceExpression([NotNull] CovaParser.SequenceExpressionContext context)
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

	abstract class Definition
	{
		public String Name { get; }
		public Definition? Parent { get; }
		public Definition(String name, Definition? parent)
		{
			Name = name;
			Parent = parent;
		}
	}
	class Package : Definition
	{
		public Package(String name) : base(name, null) {}
		public Dictionary<String, Module> Modules { get; } = new Dictionary<String, Module>();
	}
	class Module : Definition
	{
		public Module(String name, Definition? parent) : base(name, parent) {}
		public Dictionary<String, Module> Modules { get; } = new Dictionary<String, Module>();
		public Dictionary<String, Type> Types { get; } = new Dictionary<String, Type>();
	}
	class Type : Definition
	{
		public Type(String name, Definition? parent) : base(name, parent) {}
		public Dictionary<String, Type> Types { get; } = new Dictionary<String, Type>();
		public Dictionary<String, Function> Functions { get; } = new Dictionary<String, Function>();
	}
	class Function : Definition
	{
		public Function(String name, Type parent) : base(name, parent) { }
		public Dictionary<String, Statement> Statements { get; } = new Dictionary<String, Statement>();
	}
	class Statement : Definition
	{
		public Statement(String name, Definition? parent) : base(name, parent) { }
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
