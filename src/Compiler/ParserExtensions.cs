using System;
using System.Linq;
using Cova.Model;
using static Cova.Compiler.Parser.Grammar.CovaParser;
using Visibility = Cova.Symbols.Visibility;

namespace Cova
{
    static class ParserExtensions
    {
        public static Visibility ToVisibilityEnum(this VisibilityContext context) => context?.GetChild(0) switch
        {
            null => Visibility.None,
            PrivateVisibilityContext => Visibility.Private,
            ProtectedVisibilityContext => Visibility.Protected,
            InternalVisibilityContext => Visibility.Internal,
            PublicVisibilityContext => Visibility.Public,
            _ => throw new ArgumentException("Invalid child rule", nameof(context))
        };

        public static QualifiedSymbolReference ToQualifiedSymbolReference(this QualifiedTypeContext context)
        {
            var qualifiedSymbolReference = new QualifiedSymbolReference();
            foreach (var type in context.type())
                qualifiedSymbolReference.SymbolReferences.Add(type.ToSymbolReference());
            return qualifiedSymbolReference;
        }

        public static SymbolReference ToSymbolReference(this TypeContext context)
        {
            var symbolReference = new SymbolReference(context.identifier().GetText());
            if (context.typeArguments() != null)
                foreach (var typeArgument in context.typeArguments().typeArgument())
                    symbolReference.TypeParameters.Add(typeArgument.qualifiedType().ToQualifiedSymbolReference());
            return symbolReference;
        }

        public static Namespace ToNamespace(this NamespaceIdentifierContext context)
        {
            return new Namespace(context.ToDefinitionSource(), context.Identifier().GetText());
        }
    }
}