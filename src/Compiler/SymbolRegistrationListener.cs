using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Cova.Compiler.Parser.Grammar;
using Cova.Model;
using Microsoft.EntityFrameworkCore;
using static Cova.Compiler.Parser.Grammar.CovaParser;
using Type = Cova.Model.Type;

namespace Cova
{
    class SymbolRegistrationListener : CovaParserBaseListener
    {
        private const Int32 StackCapacity = 32;
        private readonly Stack<Symbol> symbols = new(StackCapacity);
        private readonly Stack<Namespace> namespaces = new(StackCapacity);
        private readonly DbConnection dbConnection;

        public SymbolRegistrationListener(Symbol rootSymbol, DbConnection dbConnection)
        {
            symbols.Push(rootSymbol);
            this.dbConnection = dbConnection;
        }

        public override void EnterUseNamespaceStatement(UseNamespaceStatementContext context)
        {
            if (symbols.Peek() is not Scope scope)
                throw new Exception();
            var qsr = new QualifiedSymbolReference();
            var identifiers = context.qualifiedIdentifier().identifier().Select(x => x.GetText());
            qsr.SymbolReferences.AddRange(identifiers.Select(x => new SymbolReference(x)));
            scope.Imported.Add(qsr);
        }

        public override void EnterNamespaceIdentifier(NamespaceIdentifierContext context)
        {
            if (symbols.Peek() is not IHasChildren<Namespace> hasNamespaces)
                throw new Exception();
            var @namespace = context.ToNamespace();
            @namespace.Parent = symbols.Peek();
            @namespace.ParentNamespace = namespaces.TryPeek(out var parentNamespace) ? parentNamespace : null;
            using var db = new Context(dbConnection);
            db.Namespaces.Add(@namespace);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                @namespace = db.Namespaces.Single(x => x.ParentNamespace == @namespace.ParentNamespace && x.Name == @namespace.Name);
            }
            namespaces.Push(@namespace);
        }

        // public override void EnterNamespaceDefinition(NamespaceDefinitionContext context)
        // {
        //     foreach (var identifier in context.qualifiedIdentifier().identifier())
        //     {
        //         if (symbols.Peek() is not IHasChildren<Namespace> hasNamespaces)
        //             throw new Exception();
        //         var @namespace = new Namespace(identifier.ToDefinitionSource(), identifier.GetText());
        //         @namespace.Parent = symbols.Peek();
        //         hasNamespaces.Children.Add(@namespace);
        //         symbols.Push(@namespace);
        //     }
        // }

        public override void EnterTypeDefinition(TypeDefinitionContext context)
        {
            if (symbols.Peek() is not IHasChildren<Type> hasTypes)
                throw new Exception();
            var type = new Type(context.ToDefinitionSource(), context.identifier().GetText());
            type.Parent = symbols.Peek();
            hasTypes.Children.Add(type);
            symbols.Push(type);
        }

        public override void EnterFunctionDefinition(FunctionDefinitionContext context)
        {
            if (symbols.Peek() is not IHasChildren<Function> hasFunctions)
                throw new Exception();
            var function = new Function(
                context.ToDefinitionSource(),
                context.identifier().GetText(),
                context.qualifiedType().ToQualifiedSymbolReference());
            function.Parent = symbols.Peek();
            hasFunctions.Children.Add(function);
            symbols.Push(function);
        }

        //public override void EnterSequenceExpression(SequenceExpressionContext context)
        //{
        //	var expressions = context.expression();
        //	var lower = expressions[0];
        //	var upper = expressions[1];
        //	var interval = expressions[3];
        //}
    }
}