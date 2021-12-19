using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Cova.Model
{
    public class Context : DbContext
    {
        readonly DbConnection dbConnection;

        public Context(DbConnection dbConnection) => this.dbConnection = dbConnection;

        public DbSet<Symbol> Symbols => Set<Symbol>();
        public DbSet<SymbolNamed> SymbolNameds => Set<SymbolNamed>();
        public DbSet<Alias> Aliases => Set<Alias>();
        public DbSet<Package> Packages => Set<Package>();
        public DbSet<Module> Modules => Set<Module>();
        public DbSet<Namespace> Namespaces => Set<Namespace>();
        public DbSet<Type> Types => Set<Type>();
        public DbSet<Field> Fields => Set<Field>();
        public DbSet<Property> Properties => Set<Property>();
        public DbSet<Function> Functions => Set<Function>();
        public DbSet<Parameter> Parameters => Set<Parameter>();
        public DbSet<Local> Locals => Set<Local>();
        public DbSet<Statement> Statements => Set<Statement>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(dbConnection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.MapTablePerType(this, entity => entity.ClrType.Assembly == typeof(Context).Assembly);
            //modelBuilder.RemovePluralizingTableNameConvention();

            modelBuilder.Entity<Namespace>().HasIndex(x => new {x.ParentNamespaceId, x.Name}).IsUnique();
            this.Database.ExecuteSqlInterpolated($"create unique index");

            // foreach (var entity in modelBuilder.Model.GetEntityTypes())
            //     foreach (var property in entity.GetProperties())
            //         property.SetColumnName(property.Name);
        }
    }
}