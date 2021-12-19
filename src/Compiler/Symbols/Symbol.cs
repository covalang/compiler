// using System;
// using System.Collections.Generic;
// using System.Collections.Immutable;
// using Cova.Symbols;
//
// namespace Compiler.Symbols
// {
// 	public class Symbol : ISymbol
// 	{
// 		public Symbol(DefinitionSource definitionSource) => DefinitionSource = definitionSource;
// 		public ISymbol? Parent { get; set; }
// 		public List<ISymbol> Children { get; } = new();
// 		public DefinitionSource DefinitionSource { get; set; }
// 	}
// 	
// 	public class Scope : Symbol, IScope
// 	{
// 		public Scope(DefinitionSource definitionSource) : base(definitionSource) {}
// 		public List<IScope> Imported { get; } = new();
// 	}
// 	
// 	public abstract class NamedScope : Scope, IHasName
// 	{
// 		protected NamedScope(DefinitionSource definitionSource, String name) : base(definitionSource) => Name = name;
// 		public String Name { get; set; }
// 	}
//
// 	public class StorageReferencing : IStorageReferencing
// 	{
// 		public Ownership Ownership { get; set; }
// 		public Visibility Visibility { get; set; }
// 		public Mutability Mutability { get; set; }
// 		public Nullability Nullability { get; set; }
// 		public StorageType StorageType { get; set; }
// 		public CyclePossibility CyclePossibility { get; set; }
// 		public InstanceDependency InstanceDependency { get; set; }
// 		public ThreadShareability ThreadShareability { get; set; }
// 	}
// 	
// 	public class Alias : Alias<ISymbol>, IAlias
// 	{
// 		public Alias(DefinitionSource definitionSource, ISymbol aliased) : base(definitionSource, aliased) {}
// 	}
//
// 	public class Alias<TSymbol> : Symbol, IAlias<TSymbol> where TSymbol : ISymbol
// 	{
// 		public Alias(DefinitionSource definitionSource, TSymbol aliased) : base(definitionSource) => Aliased = aliased;
// 		public TSymbol Aliased { get; }
// 	}
// 	
// 	public class NamespaceImport : Alias<INamespace>, INamespaceImport
// 	{
// 		public NamespaceImport(DefinitionSource definitionSource, INamespace aliased) : base(definitionSource, aliased) {}
// 	}
// 	
// 	public class TypeAlias : Alias<IType>, ITypeAlias
// 	{
// 		public TypeAlias(DefinitionSource definitionSource, IType aliased, String name) : base(definitionSource, aliased) => Name = name;
// 		public String Name { get; set; }
// 	}
// 	
// 	public class Package : Symbol, IPackage
// 	{
// 		public Package(DefinitionSource definitionSource, String name) : base(definitionSource) => Name = name;
// 		public String Name { get; set; }
// 		public List<IModule> Modules { get; } = new();
// 	}
// 	
// 	public class Module : NamedScope, IModule
// 	{
// 		public Module(DefinitionSource definitionSource, String name) : base(definitionSource, name) => Name = name;
// 		public List<IType> Types { get; } = new();
// 		public List<IFunction> Functions { get; } = new();
// 		public List<INamespace> Namespaces { get; } = new();
// 		public List<IAlias> Aliases { get; } = new();
// 	}
// 	
// 	public class Namespace : Symbol, INamespace
// 	{
// 		public Namespace(DefinitionSource definitionSource, String name) : base(definitionSource) => Name = name;
// 		public String Name { get; set; }
// 		public List<IScope> Imported { get; } = new();
// 		public List<INamespace> Namespaces { get; } = new();
// 		public List<IAlias> Aliases { get; } = new();
// 		public List<IType> Types { get; } = new();
// 		public List<IFunction> Functions { get; } = new();
// 	}
//
// 	// public abstract record Symbol(
// 	// 	String Name,
// 	// 	Visibility LocalReadVisibility,
// 	// 	Visibility LocalWriteVisibility,
// 	// 	Visibility GlobalReadVisibility,
// 	// 	Visibility GlobalWriteVisibility
// 	// ) : ISymbol;
// 	//
// 	// public sealed record FunctionSymbol(
// 	// 	String Name,
// 	// 	Visibility LocalReadVisibility,
// 	// 	Visibility LocalWriteVisibility,
// 	// 	Visibility GlobalReadVisibility,
// 	// 	Visibility GlobalWriteVisibility,
// 	// 	ImmutableArray<TypeParameterSymbol> TypeParameters,
// 	// 	ImmutableArray<ParameterSymbol> Parameters,
// 	// 	ImmutableArray<TypeSymbol> ReturnType
// 	// ) : Symbol(
// 	// 	Name,
// 	// 	LocalReadVisibility,
// 	// 	LocalWriteVisibility,
// 	// 	GlobalReadVisibility,
// 	// 	GlobalWriteVisibility
// 	// ), IFunctionSymbol;
// 	//
// 	// public abstract record TypeSymbol(String Name) : Symbol(Name), ITypeSymbol;
// 	// public sealed record FieldSymbol(String Name) : Symbol(Name), IFieldSymbol;
// 	// public sealed record PropertySymbol(String Name) : Symbol(Name), IPropertySymbol;
// 	// public sealed record ParameterSymbol(String Name) : TypeSymbol(Name), IParameterSymbol;
// 	// public sealed record TypeParameterSymbol(String Name) : TypeSymbol(Name), ITypeParameterSymbol;
// 	// public sealed record StructSymbol(String Name) : TypeSymbol(Name), IStructSymbol;
// 	// public sealed record ClassSymbol(String Name) : TypeSymbol(Name), IClassSymbol;
// }
