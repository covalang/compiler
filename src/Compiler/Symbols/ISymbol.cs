using System;
using System.Collections.Generic;
using System.Diagnostics;

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

	public class Reference<T> where T : ISymbol, IHasName
	{
		//public Reference
	}

	public interface ISymbol : IHasParent<ISymbol?>, IHasChildren<ISymbol>, IHasDefinitionSource {}

	public interface IScope : ISymbol
	{
		List<IScope> Imported { get; }
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
	public interface IModule : ISymbol, IScope, IHasName, IHasNamespaces, IHasTypes, IHasAliases, IHasFunctions {}
	public interface INamespace : ISymbol, IScope, IHasName, IHasNamespaces, IHasTypes, IHasAliases, IHasFunctions {}

	public interface IFunction : ISymbol, IScope, IHasName, IHasTypeReference, IHasParameters, IHasLocals, IHasStatements, IStorageReferencing { }
	public interface IGenericFunction : IFunction, IHasTypeParameters {}

	public interface IType : ISymbol, IScope, IHasName, IHasTypeParameters, IHasTypes, IHasFunctions, IHasFields, IHasProperties, IStorageReferencing { }
	public interface ITypeReference : IType {}
	public interface ITypeInference : IType {}

	public interface IDelegate : IType {}

	public interface ILocal : ISymbol, IHasName, IHasTypeReference, IStorageReferencing {}
	public interface IField : ISymbol, IHasName, IHasTypeReference, IStorageReferencing {}

	public interface IProperty : ISymbol, IHasName
	{
		IFunction Getter { get; set; }
		IFunction Setter { get; set; }
	}

	public interface IStatement : ISymbol {}

	public interface IParameter : ISymbol, IHasName, IHasTypeReference, IStorageReferencing {}
	public interface ITypeParameter : IParameter {}

	//public interface INamedType : IType, IHasName {}
	public interface ITrait : IType {}
	public interface IStruct : IType, IExtends<IStruct>, IImplements<ITrait> {}
	public interface IInterface : IType {}
	public interface IClass : IType, IExtends<IClass>, IImplements<IInterface> {}
	
	public interface IHasParent<TParent> { TParent Parent { get; set; } }
	public interface IHasChildren<TChild> { List<TChild> Children { get; } }
	public interface IHasDefinitionSource { DefinitionSource DefinitionSource { get; set; } }

	public interface IHasName { String Name { get; set; } }
	public interface IHasTypeReference { Reference<IType> TypeReference { get; set; } }
	public interface IHasOwnership { Ownership Ownership { get; set; } }
	public interface IHasVisibility { Visibility Visibility { get; set; } }
	public interface IHasMutability { Mutability Mutability { get; set; } }
	public interface IHasNullability { Nullability Nullability { get; set; } }
	public interface IHasStorageType { StorageType StorageType { get; set; } }
	public interface IHasCyclePossibility { CyclePossibility CyclePossibility { get; set; } }
	public interface IHasInstanceDependency { InstanceDependency InstanceDependency { get; set; } }
	public interface IHasThreadShareability { ThreadShareability ThreadShareability { get; set; } }
	
	public interface IHasModules { List<IModule> Modules { get; }}
	public interface IHasScopes { List<IScope> Scopes { get; } }
	public interface IHasAliases { List<IAlias> Aliases { get; } }
	public interface IHasNamespaces { List<INamespace> Namespaces { get; } }
	public interface IHasTypes { List<IType> Types { get; } }
	public interface IHasInterfaces { List<IType> Interfaces { get; } }
	public interface IHasTraits { List<IType> Traits { get; } }
	public interface IHasFields { List<IField> Fields { get; } }
	public interface IHasProperties { List<IProperty> Properties { get; } }
	public interface IHasFunctions { List<IFunction> Functions { get; } }
	public interface IHasParameters { List<IParameter> Parameters { get; } }
	public interface IHasLocals { List<ILocal> Locals { get; } }
	public interface IHasStatements { List<IStatement> Statements { get; } }
	public interface IHasTypeParameters { List<ITypeParameter> TypeParameters { get; } }
	
	public interface IExtends<TType> where TType : IType { List<IType> Extends { get; } }
	public interface IImplements<TType> where TType : IType { List<IType> Implements { get; } }
}