// using Cova.Model;
// using System;
// using Antlr4.Runtime;
// using Cova.Compiler.Parser.Grammar;
// using static Cova.Compiler.Parser.Grammar.CovaParser;
// using Type = Cova.Model.Type;
//
// namespace Cova
// {
// 	static class ParserToModelTransformer
// 	{
// 		public class TransformerException : Exception
// 		{
// 			public ParserRuleContext Context { get; }
// 			public TransformerException(ParserRuleContext context) => Context = context;
// 		}
// 		
// 		public static Module Process(this CompilationUnitContext context)
// 		{
// 			return context.compilationUnitBody()?.Process()
// 			       ?? throw new TransformerException(context);
// 		}
// 		
// 		public static Module Process(this CompilationUnitBodyContext context)
// 		{
// 			return new Module(context.ToDefinitionSource(), "Module");
// 		}
//
//
// 		public static Namespace Process(this NamespaceBodyContext context)
// 		{
// 			if (context.namespaceMemberDefinition() is { } namespaceMemberDefinitions)
// 			{
// 				foreach (var namespaceMemberDefinition in namespaceMemberDefinitions)
// 				{
// 					if (namespaceMemberDefinition.namespaceDefinition() is { } namespaceDefinition)
// 						namespaceDefinition.Process();
// 					else (namespaceMemberDefinition.)
// 				}
// 			}
// 			else
// 				throw new TransformerException(context);
// 		}
//
// 		public static void Process(this NamespaceMemberDefinitionContext context)
// 		{
// 			if (context.useNamespaceStatement() is { } useNamespaceStatement)
// 				Process(useNamespaceStatement);
// 			if (context.namespaceDefinition() is { } namespaceDefinition)
// 				Process(namespaceDefinition);
// 			if (context.typeDefinition() is { } typeDefinition)
// 				Process(typeDefinition);
// 			if (context.functionDefinition() is { } functionDefinition)
// 				Process(functionDefinition);
// 			if (context.statement() is { } statement)
// 				Process(statement);
// 		}
//
// 		private static void Process(this TypeDefinitionContext typeDefinition)
// 		{
// 			throw new NotImplementedException();
// 		}
//
// 		private static void Process(this NamespaceDefinitionContext namespaceDefinition)
// 		{
// 			throw new NotImplementedException();
// 		}
//
// 		private static void Process(this UseNamespaceStatementContext useNamespaceStatement)
// 		{
// 			throw new NotImplementedException();
// 		}
// 	}
// }