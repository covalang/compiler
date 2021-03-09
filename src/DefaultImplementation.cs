using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Cova
{
	public static class InterfaceImplementor
	{
		public static TInterface Create<TInterface>() where TInterface : class => StaticTypeCache<TInterface>.Constructor();

		public const String DynamicAssemblyName = "DynamicInterfaceImplementations";
		private static readonly ModuleBuilder ModuleBuilder =
			AssemblyBuilder
				.DefineDynamicAssembly(new AssemblyName(DynamicAssemblyName), AssemblyBuilderAccess.RunAndCollect)
				.DefineDynamicModule("Module");

		private static Boolean HasVirtualMethods(this Type type)
		{
			var methods =
				type
					.GetMethods(BindingFlags.Public | BindingFlags.Instance)
					.Where(x => x.IsAbstract)
					.ToHashSet();
			var propertyMethods =
				type
					.GetProperties(BindingFlags.Public | BindingFlags.Instance)
					.SelectMany(x => new[] { x.GetMethod, x.SetMethod })
					.OfType<MethodInfo>();
			methods.ExceptWith(propertyMethods);
			return methods.Any();
		}

		private static Boolean HasAbstractPropertiesWithASingleAccessor(this Type type)
		{
			return type
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.All(x => x.GetMethod?.IsAbstract == true && x.SetMethod?.IsAbstract == true);
		}

		// Using a static generic class gives us thread-safe construction of the dynamic type and its constructor call.
		private static class StaticTypeCache<TInterface> where TInterface : class
		{
			public static readonly Func<TInterface> Constructor;

			static StaticTypeCache()
			{
				if (!typeof(TInterface).IsInterface)
					throw new ArgumentException($"Generic argument `{nameof(TInterface)}` must be an interface.");
				if (typeof(TInterface).HasAbstractPropertiesWithASingleAccessor())
					throw new ArgumentException($"Generic argument `{nameof(TInterface)}` must not have properties which don't have a default implementation.");
				if (typeof(TInterface).HasVirtualMethods())
					throw new ArgumentException($"Generic argument `{nameof(TInterface)}` must not have methods unless they have a default implementation.");

				var typeBuilder = ModuleBuilder.DefineType(
					name: $"DefaultImplementation<{typeof(TInterface).Name}>",
					attr: TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.Sealed);
				typeBuilder.AddInterfaceImplementation(typeof(TInterface));

				var autoProperties =
					typeof(TInterface)
						.GetProperties(BindingFlags.Public | BindingFlags.Instance)
						.Where(x => x.GetMethod?.IsAbstract == true && x.SetMethod?.IsAbstract == true);
				var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Family, CallingConventions.Standard, Type.EmptyTypes);
				var ctorIlg = constructorBuilder.GetILGenerator();
				ctorIlg.Emit(OpCodes.Ldarg_0);
				ctorIlg.Emit(OpCodes.Call, typeof(Object).GetConstructor(Type.EmptyTypes)!);
				foreach (var property in autoProperties)
				{
					var fieldBuilder = typeBuilder.DefineField(
						fieldName: property.Name,
						type: property.PropertyType,
						attributes: FieldAttributes.Public);

					var getterBuilder = typeBuilder.DefineMethod(
						name: property.GetMethod!.Name,
						attributes: MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final,
						returnType: property.PropertyType,
						parameterTypes: Type.EmptyTypes);
					var getterIlg = getterBuilder.GetILGenerator();
					getterIlg.Emit(OpCodes.Ldarg_0);
					getterIlg.Emit(OpCodes.Ldfld, fieldBuilder);
					getterIlg.Emit(OpCodes.Ret);

					var setterBuilder = typeBuilder.DefineMethod(
						name: property.SetMethod!.Name,
						attributes: MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final,
						returnType: typeof(void),
						parameterTypes: new[] { property.PropertyType });
					var setterIlg = setterBuilder.GetILGenerator();
					setterIlg.Emit(OpCodes.Ldarg_0);
					setterIlg.Emit(OpCodes.Ldarg_1);
					setterIlg.Emit(OpCodes.Stfld, fieldBuilder);
					setterIlg.Emit(OpCodes.Ret);

					typeBuilder.DefineMethodOverride(getterBuilder, property.GetMethod);
					typeBuilder.DefineMethodOverride(setterBuilder, property.SetMethod);

					if (!property.PropertyType.IsValueType && property.PropertyType.GetConstructor(Type.EmptyTypes) is ConstructorInfo ctor)
					{
						ctorIlg.Emit(OpCodes.Ldarg_0);
						ctorIlg.Emit(OpCodes.Newobj, ctor);
						ctorIlg.Emit(OpCodes.Stfld, fieldBuilder);
					}
				}
				ctorIlg.Emit(OpCodes.Ret);

				var type = typeBuilder.CreateType()!;
				Constructor = Expression.Lambda<Func<TInterface>>(Expression.New(type)).Compile();
			}
		}
	}
}