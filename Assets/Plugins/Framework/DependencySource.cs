using System;
using Framework.Dependencies;
using UnityEngine;
using static Framework.DependencySource;
using Object = UnityEngine.Object;

namespace Framework
{
	[Flags]
	internal enum DependencySource
	{
		Local = 1 << 1,
		Children = 1 << 2,
		Parent = 1 << 3,
		Global = 1 << 4
	}

	internal readonly struct Context
	{
		public readonly Component Resolver;
		public readonly Component Component;

		public Context(Component resolver, Component component)
		{
			Resolver = resolver;
			Component = component;
		}
	}

	internal static class DependencySources
	{
		public static DependencySource All => Local | Children | Parent | Global;

		public static bool CanBeResolvedSafe(this DependencySource source, Context context, Type type) =>
			source.TryResolveInGameObject(context, type, out _) ||
			source.CanBeResolvedGloballySafe(type);

		public static bool TryResolve(this DependencySource source, Context context, Type type, out object result) =>
			source.TryResolveInGameObject(context, type, out result) ||
			source.TryResolveGlobally(type, out result);

		private static bool TryResolveInGameObject(this DependencySource source, Context context, Type type,
			out object result)
		{
			if (CanBeResolvedInGameObject(type))
			{
				if (TryResolveLocally(source, context, type, out result)) return true;
				if (TryResolveInChildren(source, context, type, out result)) return true;
				if (TryResolveInParent(source, context, type, out result)) return true;
			}

			result = default;
			return false;
		}

		private static bool CanBeResolvedInGameObject(Type type) => typeof(Component).IsAssignableFrom(type) || type.IsInterface;

		private static bool TryResolveLocally(DependencySource source, Context context, Type type, out object result)
		{
			if (source.Includes(Local) && context.Component.TryGetComponent(type, out var foundComponent))
			{
				result = foundComponent;
				return true;
			}

			result = default;
			return false;
		}

		private static bool TryResolveInChildren(DependencySource source, Context context, Type type, out object result)
		{
			if (source.Includes(Children))
			{
				var foundComponent = context.Resolver.GetComponentInChildren(type);
				if (foundComponent != null)
				{
					result = foundComponent;
					return true;
				}
			}

			result = default;
			return false;
		}

		private static bool TryResolveInParent(DependencySource source, Context context, Type type, out object result)
		{
			if (source.Includes(Parent))
			{
				var foundComponent = context.Resolver.GetComponentInParent(type);
				if (foundComponent != null)
				{
					result = foundComponent;
					return true;
				}
			}

			result = default;
			return false;
		}

		private static bool TryResolveGlobally(this DependencySource source, Type type, out object result)
		{
			var container = RootDependencyContainer.Instance;

			if (source.Includes(Global) && container && container.TryResolve(type, out result))
				return true;

			result = default;
			return false;
		}

		private static bool CanBeResolvedGloballySafe(this DependencySource source, Type type)
		{
			if (!source.Includes(Global)) return false;
			var container = Object.FindObjectOfType<RootDependencyContainer>();
			return container && container.CanBeResolvedSafe(type);
		}

		private static bool Includes(this DependencySource source, DependencySource other) => (source & other) != 0;
	}
}