using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Cova.Model
{
    public class Context : DbContext
    {
        readonly DbConnection dbConnection;
		
        public Context(DbConnection dbConnection) => this.dbConnection = dbConnection;

        public DbSet<Package> Packages => Set<Package>();
        public DbSet<Module> Modules => Set<Module>();
        public DbSet<Namespace> Namespaces => Set<Namespace>();
        public DbSet<Type> Types => Set<Type>();
        public DbSet<Field> Fields => Set<Field>();
        public DbSet<Property> Properties => Set<Property>();
        public DbSet<Function> Functions => Set<Function>();
        public DbSet<Statement> Statements => Set<Statement>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(dbConnection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.RemovePluralizingTableNameConvention();
        }
    }
}