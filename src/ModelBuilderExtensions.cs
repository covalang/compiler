using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Cova
{
	public static class ModelBuilderExtensions
	{
		public static void MapTablePerType(this ModelBuilder modelBuilder, DbContext context)
		{
			var entitySets =
				from x in context.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
				where x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition().IsAssignableTo(typeof(DbSet<>))
				select (entityType: x.PropertyType.GetGenericArguments().Single(), name: x.Name);

			foreach (var (entityType, name) in entitySets)
				modelBuilder.Entity(entityType).ToTable(name);
		}
	}
}