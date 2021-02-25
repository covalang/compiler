using System;
using System.Collections.Immutable;

namespace Compiler.Symbols
{
	public enum Ownership : Byte { Unique, Shared }
	public enum Visibility : Byte { None, Private, Protected, Internal, Public }
	public enum Mutability : Byte { Immutable, Mutable }
	public enum Nullability : Byte { None, Nullable }
	public enum StorageType : Byte { Static, Instance }
	public enum CyclePossibility : Byte { Unknown, None, Some }
	public enum InstanceDependency : Byte { Value, Reference } // Independent, Interdependent
	public enum ThreadShareability : Byte { Local, Global }

	public struct Location
	{
		public UInt32 Offset;
		public UInt32 Line;
		public UInt32 Column;
	}

	public interface ISymbol
	{
		String Name { get; }
		Ownership Ownership { get; }
		Visibility Visibility { get; }
		Mutability Mutability { get; }
		Nullability Nullability { get; }
		StorageType StorageType { get; }
		CyclePossibility CyclePossibility { get; }
		InstanceDependency InstanceDependency { get; }
		ThreadShareability ThreadShareability { get; }
		ImmutableArray<(Location start, Location stop)> LocationPairs { get; }
	}

	public interface IAlias : ISymbol
	{
		ISymbol Aliased { get; }
	}

	public interface IFunctionSymbol : ISymbol
	{
		ImmutableArray<ITypeParameterSymbol> TypeParameters { get; }
		ImmutableArray<IParameterSymbol> Parameters { get; }
		ImmutableArray<ITypeSymbol> ReturnType { get; }
	}

	public interface ITypeSymbol : ISymbol
	{
		ImmutableArray<ITypeParameterSymbol> TypeParameters { get; }
		ImmutableArray<INamespaceSymbol> Namespaces { get; }
	}

	public interface ILocalSymbol : ISymbol { }
	public interface IFieldSymbol : ISymbol { }
	public interface IPropertySymbol : ISymbol { }
	
	public interface IParameterSymbol : ITypeSymbol { }
	public interface ITypeParameterSymbol : ITypeSymbol { }
	
	public interface INamespaceSymbol : ISymbol { }
	public interface IStructSymbol : ITypeSymbol { }
	public interface IClassSymbol : ITypeSymbol { }
	
	// interface IFileSymbol { }
}