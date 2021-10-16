using System;
using System.Collections.Immutable;

namespace Compiler.Symbols
{
	// public abstract record Symbol(
	// 	String Name,
	// 	Visibility LocalReadVisibility,
	// 	Visibility LocalWriteVisibility,
	// 	Visibility GlobalReadVisibility,
	// 	Visibility GlobalWriteVisibility
	// ) : ISymbol;
	//
	// public sealed record FunctionSymbol(
	// 	String Name,
	// 	Visibility LocalReadVisibility,
	// 	Visibility LocalWriteVisibility,
	// 	Visibility GlobalReadVisibility,
	// 	Visibility GlobalWriteVisibility,
	// 	ImmutableArray<TypeParameterSymbol> TypeParameters,
	// 	ImmutableArray<ParameterSymbol> Parameters,
	// 	ImmutableArray<TypeSymbol> ReturnType
	// ) : Symbol(
	// 	Name,
	// 	LocalReadVisibility,
	// 	LocalWriteVisibility,
	// 	GlobalReadVisibility,
	// 	GlobalWriteVisibility
	// ), IFunctionSymbol;
	//
	// public abstract record TypeSymbol(String Name) : Symbol(Name), ITypeSymbol;
	// public sealed record FieldSymbol(String Name) : Symbol(Name), IFieldSymbol;
	// public sealed record PropertySymbol(String Name) : Symbol(Name), IPropertySymbol;
	// public sealed record ParameterSymbol(String Name) : TypeSymbol(Name), IParameterSymbol;
	// public sealed record TypeParameterSymbol(String Name) : TypeSymbol(Name), ITypeParameterSymbol;
	// public sealed record StructSymbol(String Name) : TypeSymbol(Name), IStructSymbol;
	// public sealed record ClassSymbol(String Name) : TypeSymbol(Name), IClassSymbol;
}
