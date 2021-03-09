using System;

namespace Cova.Symbols
{
	public enum Ownership : Byte { Unique, Shared }
	public enum Visibility : Byte { None, Private, Protected, Internal, Public }
	public enum Mutability : Byte { Immutable, Mutable }
	public enum Nullability : Byte { None, Nullable }
	public enum StorageType : Byte { Static, Instance }
	public enum CyclePossibility : Byte { Unknown, None, Some }
	public enum InstanceDependency : Byte { Value, Reference } // Independent, Interdependent
	public enum ThreadShareability : Byte { Local, Global }

	public interface ISymbol : IHasParent<ISymbol>, IHasChildren<ISymbol>
	{
		DefinitionSource DefinitionSource { get; set; }
	}

	public interface IScope : ISymbol
	{
		OrderedSet<IScope> Children { get; set; }
		OrderedSet<IScope> Imported { get; set; }
		OrderedSet<ISymbol> Symbols { get; set; }
		IScope? Parent { get; set; }
	}

	public interface IStorageReferencing
		: IHasOwnership
		, IHasVisibility
		, IHasMutability
		, IHasNullability
		, IHasStorageType
		, IHasCyclePossibility
		, IHasInstanceDependency
		, IHasThreadShareability
	{ }

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

	public interface IType : ISymbol, IScope, IHasTypes, IHasFunctions, IHasFields, IHasProperties, IExtends, IImplements {}
	public interface IGenericType : IType, IHasTypeParameters {}
	
	public interface IDelegate : IType {}

	public interface ILocal : ISymbol, IHasName, IStorageReferencing {}
	public interface IField : ISymbol, IHasName, IStorageReferencing {}

	public interface IProperty : ISymbol, IHasName
	{
		IFunction Getter { get; set; }
		IFunction Setter { get; set; }
	}
	public interface IStatement : ISymbol {}

	public interface IParameter : ISymbol, IHasName, IHasType {}
	public interface ITypeParameter : IParameter {}
	
	public interface IInterface : IType, IHasName {}
	public interface ITrait : IType, IHasName {}
	public interface IStruct : IType, IHasName {}
	public interface IClass : IType, IHasName {}
	
	public interface IHasParent<TParent> { TParent? Parent { get; set; } }
	public interface IHasChildren<TChild> { OrderedSet<TChild> Children { get; set; } }

	public interface IHasName { String Name { get; set; } }
	public interface IHasType { IType Type { get; set; } }
	public interface IHasOwnership { Ownership Ownership { get; set; } }
	public interface IHasVisibility { Visibility Visibility { get; set; } }
	public interface IHasMutability { Mutability Mutability { get; set; } }
	public interface IHasNullability { Nullability Nullability { get; set; } }
	public interface IHasStorageType { StorageType StorageType { get; set; } }
	public interface IHasCyclePossibility { CyclePossibility CyclePossibility { get; set; } }
	public interface IHasInstanceDependency { InstanceDependency InstanceDependency { get; set; } }
	public interface IHasThreadShareability { ThreadShareability ThreadShareability { get; set; } }
	
	public interface IHasModules { OrderedSet<IModule> Modules { get; set; } }
	public interface IHasScopes { OrderedSet<IScope> Scopes { get; set; } }
	public interface IHasAliases { OrderedSet<IAlias> Aliases { get; set; } }
	public interface IHasNamespaces { OrderedSet<INamespace> Namespaces { get; set; } }
	public interface IHasTypes { OrderedSet<IType> Types { get; set; } }
	public interface IHasInterfaces { OrderedSet<IType> Interfaces { get; set; } }
	public interface IHasTraits { OrderedSet<IType> Traits { get; set; } }
	public interface IHasFields { OrderedSet<IField> Fields { get; set; } }
	public interface IHasProperties { OrderedSet<IProperty> Properties { get; set; } }
	public interface IHasFunctions { OrderedSet<IFunction> Functions { get; set; } }
	public interface IHasParameters { OrderedSet<IParameter> Parameters { get; set; } }
	public interface IHasLocals { OrderedSet<ILocal> Locals { get; set; } }
	public interface IHasStatements { OrderedSet<IStatement> Statements { get; set; } }
	public interface IHasTypeParameters { OrderedSet<ITypeParameter> TypeParameters { get; set; } }
	
	public interface IExtends { OrderedSet<IType> Extends { get; set; } }
	public interface IImplements { OrderedSet<IType> Implements { get; set; } }
}