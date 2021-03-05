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

	public interface ISymbol
	{
		DefinitionSource DefinitionSource { get; }
	}

	public interface IScope
	{
		OrderedSet<IScope> Children { get; set; }
		OrderedSet<IScope> Imported { get; set; }
		OrderedSet<ISymbol> Symbols { get; set; }
		IScope? Parent { get; set; }
	}

	public interface IStorageReferencing
	{
		Ownership Ownership { get; }
		Visibility Visibility { get; }
		Mutability Mutability { get; }
		Nullability Nullability { get; }
		StorageType StorageType { get; }
		CyclePossibility CyclePossibility { get; }
		InstanceDependency InstanceDependency { get; }
		ThreadShareability ThreadShareability { get; }
	}

	public interface IAlias : IAlias<ISymbol> {}
	public interface IAlias<out TSymbol> : ISymbol where TSymbol : ISymbol
	{
		TSymbol Aliased { get; }
	}
	public interface INamespaceImport : IAlias<INamespace> {}
	public interface ITypeAlias : IAlias<IType>, IHasName {}

	public interface IPackage : ISymbol, IHasName, IHasModules {}
	public interface IModule : ISymbol, IHasName, IHasTypes, IHasFunctions, IHasNamespaces, IHasAliases {}
	public interface INamespace : ISymbol, IScope, IHasName, IHasNamespaces, IHasAliases, IHasTypes, IHasFunctions {}

	public interface IFunction : ISymbol, IScope, IHasName, IHasType, IHasParameters, IHasLocals, IHasStatements {}
	public interface IGenericFunction : IFunction, IHasTypeParameters {}

	public interface IType : ISymbol, IScope, IHasTypes, IHasFunctions, IHasFields, IHasProperties
	{
		OrderedSet<IInterface> Extends { get; }
		OrderedSet<IType> Implements { get; }
	}
	public interface IGenericType : IType, IHasTypeParameters {}
	
	public interface IDelegate : IType {}

	public interface ILocal : ISymbol, IHasName, IStorageReferencing {}
	public interface IField : ISymbol, IHasName, IStorageReferencing {}

	public interface IProperty : ISymbol, IHasName
	{
		IFunction Getter { get; }
		IFunction Setter { get; }
	}
	public interface IStatement : ISymbol {}

	public interface IParameter : ISymbol, IHasName, IHasType {}
	public interface ITypeParameter : IParameter {}
	
	public interface IInterface : ISymbol, IScope, IHasName {}
	public interface ITrait : ISymbol, IScope, IHasName {}
	public interface IStruct : IHasName, IType, IHasTypes, IHasFunctions {}
	public interface IClass : IHasName, IType, IHasTypes, IHasFunctions {}
	
	public interface IHasName { String Name { get; } }
	public interface IHasType { IType Type { get; } }
	public interface IHasOwnership { Ownership Ownership { get; } }
	public interface IHasVisibility { Visibility Visibility { get; } }
	public interface IHasMutability { Mutability Mutability { get; } }
	public interface IHasNullability { Nullability Nullability { get; } }
	public interface IHasStorageType { StorageType StorageType { get; } }
	public interface IHasCyclePossibility { CyclePossibility CyclePossibility { get; } }
	public interface IHasInstanceDependency { InstanceDependency InstanceDependency { get; } }
	public interface IHasThreadShareability { ThreadShareability ThreadShareability { get; } }
	
	public interface IHasModules { OrderedSet<IModule> Modules { get; } }
	public interface IHasScopes { OrderedSet<IScope> Scopes { get; } }
	public interface IHasAliases { OrderedSet<IAlias> Aliases { get; } }
	public interface IHasNamespaces { OrderedSet<INamespace> Namespaces { get; } }
	public interface IHasTypes { OrderedSet<IType> Types { get; } }
	public interface IHasInterfaces { OrderedSet<IType> Interfaces { get; } }
	public interface IHasTraits { OrderedSet<IType> Traits { get; } }
	public interface IHasFields { OrderedSet<IField> Fields { get; } }
	public interface IHasProperties { OrderedSet<IProperty> Properties { get; } }
	public interface IHasFunctions { OrderedSet<IFunction> Functions { get; } }
	public interface IHasParameters { OrderedSet<IParameter> Parameters { get; } }
	public interface IHasLocals { OrderedSet<ILocal> Locals { get; } }
	public interface IHasStatements { OrderedSet<IStatement> Statements { get; } }
	public interface IHasTypeParameters { OrderedSet<ITypeParameter> TypeParameters { get; } }
	
	public interface IExtends { OrderedSet<IType> BaseTypes { get; } }
}