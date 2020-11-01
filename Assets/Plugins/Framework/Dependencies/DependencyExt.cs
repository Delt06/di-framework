using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Framework.Core;
using Framework.Dependencies.Exceptions;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Dependencies
{
	internal static class DependencyExt
	{
		public static void ResolveDependencies([NotNull] this Component context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			var type = context.GetType();

			foreach (var field in type.GetDependencyFields())
			{
				var attribute = field.GetCustomAttribute<DependencyAttribute>();
				var value = Resolve(context, field.FieldType, attribute.Source);
				field.SetValue(context, value);
			}

			foreach (var property in type.GetDependencyProperties())
			{
				var attribute = property.GetCustomAttribute<DependencyAttribute>();
				var value = Resolve(context, property.PropertyType, attribute.Source);

				if (property.CanWrite)
					property.SetValue(context, value);
				else
					throw new InvalidOperationException($"Property {property} of {type} has no setter.");
			}
		}

		private static void RequireFromEntity([NotNull] this Component context, [NotNull] Type type,
			out Component component)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			if (type == null) throw new ArgumentNullException(nameof(type));

			context.RequireInParent(out IEntity entity);
			entity.gameObject.RequireInChildren(type, out component);
		}

		private static void Require([NotNull] this Component context, [NotNull] Type type, out Component component)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			if (type == null) throw new ArgumentNullException(nameof(type));

			if (context.TryGetComponent(type, out component)) return;

			throw new ComponentResolutionError(context, type);
		}

		private static void RequireInParent<T>([NotNull] this Component context, out T component) where T : class
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			component = context.GetComponentInParent<T>();
			if (component != null) return;

			throw new ParentComponentResolutionError(context, typeof(T));
		}

		private static void RequireInParent([NotNull] this Component context, [NotNull] Type type,
			out Component component)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			if (type == null) throw new ArgumentNullException(nameof(type));

			component = context.GetComponentInParent(type);
			if (component != null) return;

			throw new ParentComponentResolutionError(context, type);
		}

		private static void RequireInChildren([NotNull] this Component context, [NotNull] Type type,
			out Component component)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			if (type == null) throw new ArgumentNullException(nameof(type));

			component = context.GetComponentInChildren(type);
			if (component != null) return;

			throw new ChildrenComponentResolutionError(context, type);
		}

		private static void RequireInChildren([NotNull] this GameObject context, [NotNull] Type type,
			out Component component)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			if (type == null) throw new ArgumentNullException(nameof(type));

			component = context.GetComponentInChildren(type);
			if (component != null) return;

			throw new ChildrenComponentResolutionError(context, type);
		}

		private static IEnumerable<FieldInfo> GetDependencyFields(this Type type)
		{
			return type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
				.Where(f => Attribute.IsDefined(f, typeof(DependencyAttribute)));
		}

		private static IEnumerable<PropertyInfo> GetDependencyProperties(this Type type)
		{
			return type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
				.Where(f => Attribute.IsDefined(f, typeof(DependencyAttribute)));
		}

		private static object Resolve(Component context, Type type, Source source)
		{
			switch (source)
			{
				case Source.Local:
				{
					context.Require(type, out var component);
					return component;
				}

				case Source.Parents:
				{
					context.RequireInParent(type, out var component);
					return component;
				}

				case Source.Children:
				{
					context.RequireInChildren(type, out var component);
					return component;
				}

				case Source.Global:
				{
					var entity = context.GetComponentInParent<IEntity>();
					if (entity != null)
						return entity.ResolveGlobal(type);

					var container = Object.FindObjectOfType<RootDependencyContainer>();
					if (container == null)
						goto default;

					return container.Resolve(type);
				}

				case Source.Entity:
				{
					context.RequireFromEntity(type, out var component);
					return component;
				}

				default: throw new ArgumentOutOfRangeException(nameof(source), source, null);
			}
		}

		public static Exception NewGlobalDependencyIllegalTypeException(Type type, object context) =>
			throw new InvalidOperationException(
				$"Global dependencies must derive from {typeof(Object)} but the type was {type}. Context: {context}.");
	}
}