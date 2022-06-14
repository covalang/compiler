using System;
using System.Linq;
using System.Reflection;
using Cova.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cova
{
	public static class ModelBuilderExtensions
	{
		public static void MapTablePerType(this ModelBuilder modelBuilder, DbContext context, Func<IMutableEntityType, Boolean> entityPredicate)
		{
			// var entitySets =
			// 	from x in context.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
			// 	where x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition().IsAssignableTo(typeof(DbSet<>))
			// 	select (entityType: x.PropertyType.GetGenericArguments().Single(), name: x.Name);
			//
			// foreach (var (entityType, name) in entitySets)
			// 	modelBuilder.Entity(entityType);
			foreach (var entity in modelBuilder.Model.GetEntityTypes())
				if (entityPredicate(entity))
					modelBuilder.Entity(entity.ClrType).ToTable(entity.DisplayName());
		}
		
		public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
		{
			foreach (var entity in modelBuilder.Model.GetEntityTypes())
				entity.SetTableName(entity.DisplayName());
		}
	}
}