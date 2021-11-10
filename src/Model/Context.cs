using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Cova.Model
{
    public class Context : DbContext
    {
        readonly DbConnection dbConnection;
		
        public Context(DbConnection dbConnection) => this.dbConnection = dbConnection;

        public DbSet<Package> Packages => Set<Package>();
        public DbSet<Module> Modules => Set<Module>();
        public DbSet<Type> Types => Set<Type>();
        public DbSet<Function> Functions => Set<Function>();
        public DbSet<Statement> Statements => Set<Statement>();

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite(dbConnection);//("DataSource=file:memdb1?mode=memory&cache=shared");//"Data Source=Graph.db;Cache=Shared");
        //protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.MapTablePerType(this);
    }
    
    public enum Ownership : Byte { Unique, Shared }
    public enum Visibility : Byte { None, Private, Protected, Internal, Public }
    public enum Mutability : Byte { Immutable, Mutable }
    public enum Nullability : Byte { None, Nullable }
    public enum StorageType : Byte { Static, Instance }
    public enum CyclePossibility : Byte { Unknown, None, Some }
    public enum InstanceDependency : Byte { Value, Reference } // Independent, Interdependent
    public enum ThreadShareability : Byte { Local, Global }

    public class Statement : Symbol
    {
        protected Statement() {}
        public Statement(DefinitionSource definitionSource) : base(definitionSource) {}
    }

    public class DefinitionSource : EntityBase
    {
        protected DefinitionSource() {}
        public DefinitionSource(SourceLocation begin, SourceLocation end)
        {
            Begin = begin;
            End = end;
        }

        public SourceLocation Begin { get; set; } = null!;
        public SourceLocation End { get; set; } = null!;
    }

    public class SourceLocation : EntityBase
    {
        protected SourceLocation() {}
        public SourceLocation(String path) => Path = path;
        public String Path { get; set; } = null!;
        public UInt32 Offset { get; set; }
        public UInt32 Line { get; set; }
        public UInt32 Column { get; set; }
    }
    
    public class Parameter : Symbol
    {
        protected Parameter() {}
        public Parameter(DefinitionSource definitionSource, String name, TypeReference typeReference) : base(definitionSource)
        {
            Name = name;
            TypeReference = typeReference;
        }

        public String Name { get; set; } = null!;
        public TypeReference TypeReference { get; set; } = null!;
        public Ownership Ownership { get; set; }
        public Visibility Visibility { get; set; }
        public Mutability Mutability { get; set; }
        public Nullability Nullability { get; set; }
        public StorageType StorageType { get; set; }
        public CyclePossibility CyclePossibility { get; set; }
        public InstanceDependency InstanceDependency { get; set; }
        public ThreadShareability ThreadShareability { get; set; }
    }

    public class TypeReference : EntityBase
    {
        protected TypeReference() {}
        public TypeReference(String typeName) => TypeName = typeName;

        public String TypeName { get; set; } = null!;
        public List<String> TypeParameterNames { get; } = new();
    }

    public class Field : Symbol
    {
        protected Field() {}
        public Field(DefinitionSource definitionSource, String name, TypeReference typeReference) : base(definitionSource)
        {
            Name = name;
            TypeReference = typeReference;
        }

        public String Name { get; set; } = null!;
        public TypeReference TypeReference { get; set; } = null!;
    }
}