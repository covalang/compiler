using System;
using System.Collections.Immutable;

namespace Compiler.Symbols
{
	public enum Visibility { None, Private, Protected, Internal, ProtectedAndInternal, ProtectedOrInternal, Public }
	public enum InstanceDependence { Value, Reference }
	public enum Mutability { Immutable, Mutable }
	public enum Nullability { NonNullable, Nullable }
	public enum Sharability { Local, Global }

	public interface ISymbol
	{
		String Name { get; }
		Visibility Visibility { get; }
		InstanceDependence InstanceDependence { get; }
		Mutability Mutability { get; }
		Nullability Nullability { get; }
		Sharability Sharability { get; }
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