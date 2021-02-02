using System;
using System.Collections.Immutable;

namespace Compiler.Symbols
{
	public enum Visibility { None, Private, Protected, Internal, Public };
	
	public interface ISymbol
	{
		String Name { get; }
		Visibility LocalReadVisibility { get; }
		Visibility LocalWriteVisibility { get; }
		Visibility GlobalReadVisibility { get; }
		Visibility GlobalWriteVisibility { get; }
	}

	public interface IAlias : ISymbol { }
	public interface IMemoryAlias : IAlias { }

	public interface IFunctionSymbol : ISymbol
	{
		ImmutableArray<TypeParameterSymbol> TypeParameters { get; }
		ImmutableArray<ParameterSymbol> Parameters { get; }
		ImmutableArray<TypeSymbol> ReturnType { get; }
	}

	public interface ITypeSymbol : ISymbol
	{
		ImmutableArray<TypeParameterSymbol> TypeParameters { get; }
	}

	public interface ILocalSymbol : ISymbol { }
	public interface IFieldSymbol : ISymbol { }
	public interface IPropertySymbol : ISymbol { }
	
	public interface IParameterSymbol : ITypeSymbol { }
	public interface ITypeParameterSymbol : ITypeSymbol { }
	public interface IStructSymbol : ITypeSymbol { }
	public interface IClassSymbol : ITypeSymbol { }
	
	// interface IFileSymbol { }
}