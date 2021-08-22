using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Cova
{
	public static class InterfaceImplementor
	{
		public static TInterface Create<TInterface>() where TInterface : class => StaticTypeCache<TInterface>.Constructor();
		public static TInterface CreateAndInitialize<TInterface>() where TInterface : class => StaticTypeCache<TInterface>.InitializingConstructor();

		public static T As<T>(this T @this) where T : class => @this;

		public const String DynamicAssemblyName = "DynamicInterfaceImplementations";

		private static readonly ModuleBuilder ModuleBuilder = AssemblyBuilder
			.DefineDynamicAssembly(new AssemblyName(DynamicAssemblyName), AssemblyBuilderAccess.RunAndCollect)
			.DefineDynamicModule("Module");

		private const BindingFlags MemberBindingFlags =
			BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

		private const MethodAttributes PropertyAccessorMethodAttributes =
			MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig |
			MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;

		private static Boolean HasVirtualMethods(this Type type)
		{
			var methods = type
				.GetMethods(MemberBindingFlags)
				.Where(x => x.IsAbstract)
				.ToHashSet();
			var propertyMethods = type
				.GetProperties(MemberBindingFlags)
				.SelectMany(x => new[] { x.GetMethod, x.SetMethod })
				.OfType<MethodInfo>();
			methods.ExceptWith(propertyMethods);
			return methods.Any();
		}

		private static Boolean HasAnyAbstractPropertiesWithASingleAbstractAccessor(this Type type)
		{
			return type.GetImplementableProperties().Any(x =>
				(x.GetMethod, x.SetMethod) is (null, MethodInfo { IsAbstract: true }) or (MethodInfo { IsAbstract: true }, null)
			);
		}

		private static IEnumerable<PropertyInfo> GetImplementableProperties(this Type type)
		{
			var interfaces = type.AsDepthFirstEnumerable(x => x.GetInterfaces());
			return interfaces.SelectMany(x => x.GetProperties(MemberBindingFlags));
		}

		private static IEnumerable<T> AsDepthFirstEnumerable<T>(this T head, Func<T, IEnumerable<T>> childrenFunc)
		{
			yield return head;
			foreach (var node in childrenFunc(head))
				foreach (var child in AsDepthFirstEnumerable(node, childrenFunc))
					yield return child;
		}

		// Using a static generic class gives us thread-safe construction and caching of the dynamic type and its constructor call.
		private static class StaticTypeCache<TInterface> where TInterface : class
		{
			public static readonly Func<TInterface> Constructor;
			public static readonly Func<TInterface> InitializingConstructor;

			static StaticTypeCache()
			{
				if (!typeof(TInterface).IsInterface)
					throw new ArgumentException($"Generic argument `{nameof(TInterface)}` must be an interface.");
				if (typeof(TInterface).HasAnyAbstractPropertiesWithASingleAbstractAccessor())
					throw new ArgumentException($"Generic argument `{nameof(TInterface)}` must not have properties which don't have a default implementation.");
				if (typeof(TInterface).HasVirtualMethods())
					throw new ArgumentException($"Generic argument `{nameof(TInterface)}` must not have methods unless they have a default implementation.");

				var typeBuilder = ModuleBuilder.DefineType(
					name: $"<{typeof(TInterface).Name}>",
					attr: TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.Sealed);
				typeBuilder.AddInterfaceImplementation(typeof(TInterface));
				var constructorBuilder = typeBuilder.DefineDefaultConstructor(MethodAttributes.Family);

				var initializerBuilder = typeBuilder.DefineMethod(
					name: "<CreateAndInitialize>",
					attributes: MethodAttributes.Family | MethodAttributes.Static,
					returnType: typeof(TInterface),
					parameterTypes: Type.EmptyTypes);
				var initIlg = initializerBuilder.GetILGenerator();
				initIlg.Emit(OpCodes.Newobj, constructorBuilder);

				var autoProperties =
					typeof(TInterface)
						.GetImplementableProperties()
						.Where(x => x.GetMethod?.IsAbstract == true && x.SetMethod?.IsAbstract == true)
						.Distinct(InterfacePropertyInfoEqualityComparer.Default);

				foreach (var property in autoProperties)
				{
					var fieldBuilder = typeBuilder.DefineField(
						fieldName: $"<{property.Name}>k__BackingField",
						type: property.PropertyType,
						attributes: FieldAttributes.Private);
					fieldBuilder.SetCustomAttribute(
						new CustomAttributeBuilder(
							typeof(DebuggerBrowsableAttribute).GetConstructor(new [] { typeof(DebuggerBrowsableState) })!, new Object[] { DebuggerBrowsableState.Never }
						)
					);

					var getterBuilder = typeBuilder.DefineMethod(
						name: property.GetMethod?.Name ?? throw new Exception("Getter must be defined."),
						attributes: PropertyAccessorMethodAttributes,
						returnType: property.PropertyType,
						parameterTypes: Type.EmptyTypes);
					var getterIlg = getterBuilder.GetILGenerator();
					getterIlg.Emit(OpCodes.Ldarg_0);
					getterIlg.Emit(OpCodes.Ldfld, fieldBuilder);
					getterIlg.Emit(OpCodes.Ret);
					//typeBuilder.DefineMethodOverride(getterBuilder, property.GetMethod);

					var setterBuilder = typeBuilder.DefineMethod(
						name: property.SetMethod?.Name ?? throw new Exception("Setter must be defined."),
						attributes: PropertyAccessorMethodAttributes,
						returnType: typeof(void),
						parameterTypes: new[] { property.PropertyType });
					var setterIlg = setterBuilder.GetILGenerator();
					setterIlg.Emit(OpCodes.Ldarg_0);
					setterIlg.Emit(OpCodes.Ldarg_1);
					setterIlg.Emit(OpCodes.Stfld, fieldBuilder);
					setterIlg.Emit(OpCodes.Ret);
					//typeBuilder.DefineMethodOverride(setterBuilder, property.SetMethod);

					var propertyBuilder = typeBuilder.DefineProperty(
						name: property.Name,
						attributes: PropertyAttributes.None,
						returnType: property.PropertyType,
						parameterTypes: Type.EmptyTypes);
					propertyBuilder.SetGetMethod(getterBuilder);
					propertyBuilder.SetSetMethod(setterBuilder);

					if (!property.PropertyType.IsValueType && property.PropertyType.GetConstructor(Type.EmptyTypes) is ConstructorInfo ctor)
					{
						initIlg.Emit(OpCodes.Dup);
						initIlg.Emit(OpCodes.Newobj, ctor);
						initIlg.Emit(OpCodes.Stfld, fieldBuilder);
					}
				}
				initIlg.Emit(OpCodes.Ret);

				var type = typeBuilder.CreateType() ?? throw new Exception("Type creation failed.");
				Constructor = Expression.Lambda<Func<TInterface>>(Expression.New(type)).Compile();
				var initializerMethod = type.GetMethod(initializerBuilder.Name, BindingFlags.NonPublic | BindingFlags.Static) ?? throw new Exception("Method not found.");
				InitializingConstructor = initializerMethod.CreateDelegate<Func<TInterface>>();
			}

			private sealed class InterfacePropertyInfoEqualityComparer : EqualityComparer<PropertyInfo>
			{
				public override Boolean Equals(PropertyInfo? x, PropertyInfo? y)
				{
					if (ReferenceEquals(x, y)) // considered equal if both are null or both refer to same single instance
						return true;
					if (x == null || y == null) // not equal if one is null
						return false;
					return x.PropertyType == y.PropertyType && x.Name == y.Name; // equal if type and name match
				}

				public override Int32 GetHashCode([DisallowNull] PropertyInfo obj) => HashCode.Combine(obj.PropertyType.FullName, obj.Name);

				public static new InterfacePropertyInfoEqualityComparer Default { get; } = new InterfacePropertyInfoEqualityComparer();

				private InterfacePropertyInfoEqualityComparer() { }
			}
		}
	}
}