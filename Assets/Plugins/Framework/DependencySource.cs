using System;
using Framework.Dependencies;
using UnityEngine;
using static Framework.DependencySource;

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

		public static bool TryResolve(this DependencySource source, Context context, Type type, out object result)
		{
			if (IsComponent(type))
			{
				if (TryResolveLocally(source, context, type, out result)) return true;
				if (TryResolveInChildren(source, context, type, out result)) return true;
				if (TryResolveInParent(source, context, type, out result)) return true;
			}

			return TryResolveGlobally(source, type, out result);
		}

		private static bool IsComponent(Type type) => typeof(Component).IsAssignableFrom(type);

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

		private static bool TryResolveGlobally(DependencySource source, Type type, out object result)
		{
			if (source.Includes(Global) && RootDependencyContainer.Instance.TryResolve(type, out result)) 
				return true;

			result = default;
			return false;
		}

		private static bool Includes(this DependencySource source, DependencySource other) => (source & other) != 0;
	}
}