using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Cova
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
		protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.MapTablePerType(this);
	}
}