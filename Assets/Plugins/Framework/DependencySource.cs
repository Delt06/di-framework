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
			if (typeof(Component).IsAssignableFrom(type))
			{
				if (source.Includes(Local))
					if (context.Component.TryGetComponent(type, out var foundComponent))
					{
						result = foundComponent;
						return true;
					}

				if (source.Includes(Children))
				{
					var foundComponent = context.Resolver.GetComponentInChildren(type);
					if (foundComponent != null)
					{
						result = foundComponent;
						return true;
					}
				}

				if (source.Includes(Parent))
				{
					var foundComponent = context.Resolver.GetComponentInParent(type);
					if (foundComponent != null)
					{
						result = foundComponent;
						return true;
					}
				}
			}

			if (source.Includes(Global))
				if (RootDependencyContainer.Instance.TryResolve(type, out result))
					return true;

			result = default;
			return false;
		}

		private static bool Includes(this DependencySource source, DependencySource other) => (source & other) != 0;
	}
}