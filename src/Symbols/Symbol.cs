using System;
using System.Collections.Immutable;

namespace Compiler.Symbols
{
	public abstract record Symbol(String Name);

	public sealed record FunctionSymbol(
		String Name,
		ImmutableArray<TypeParameterSymbol> TypeParameters,
		ImmutableArray<ParameterSymbol> Parameters,
		ImmutableArray<TypeSymbol> ReturnType
	) : Symbol(Name);

	public abstract record TypeSymbol(String Name) : Symbol(Name);
	public sealed record FieldSymbol(String Name) : Symbol(Name);
	public sealed record PropertySymbol(String Name) : Symbol(Name);
	public sealed record ParameterSymbol(String Name) : TypeSymbol(Name);
	public sealed record TypeParameterSymbol(String Name) : TypeSymbol(Name);
	public sealed record StructSymbol(String Name) : TypeSymbol(Name);
	public sealed record ClassSymbol(String Name) : TypeSymbol(Name);
}
