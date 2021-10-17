using System;
using Cova.Symbols;

using static Cova.Compiler.Parser.Grammar.CovaParser;

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
    
    //public static Ownership ToOwnershipEnum(this )
}