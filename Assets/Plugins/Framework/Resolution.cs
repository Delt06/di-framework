using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace Framework
{
	public static class Resolution
	{
		public static IEnumerable<Type> GetAllDependenciesOf([NotNull] MonoBehaviour component)
		{
			if (component == null) throw new ArgumentNullException(nameof(component));
			return GetMethodsIn(component)
				.SelectMany(m => m.GetParameters())
				.Select(p => p.ParameterType)
				.Distinct();
		}

		public static bool IsInjectable([NotNull] MonoBehaviour component)
		{
			if (component == null) throw new ArgumentNullException(nameof(component));
			var methods = GetMethodsIn(component);

			foreach (var method in methods)
			{
				var parameters = method.GetParameters();
				if (!parameters.AreInjectable())
					return false;
			}

			return true;
		}

		public static bool AreInjectable([NotNull] this ParameterInfo[] parameters)
		{
			if (parameters == null) throw new ArgumentNullException(nameof(parameters));

			foreach (var parameter in parameters)
			{
				if (!parameter.IsInjectable())
					return false;
			}

			return true;
		}

		private static bool IsInjectable([NotNull] this ParameterInfo parameter)
		{
			if (parameter == null) throw new ArgumentNullException(nameof(parameter));
			var parameterType = parameter.ParameterType;
			return !parameterType.IsValueType && !parameter.IsOut &&
			       !parameter.IsIn && !parameterType.IsByRef;
		}

		public static IEnumerable<(MonoBehaviour component, int depth)> GetAffectedComponents(Transform root,
			int depth = 0)
		{
			var components = root.GetComponents<MonoBehaviour>();

			foreach (var component in components)
			{
				if (component is Resolver) continue;
				yield return (component, depth);
			}

			foreach (Transform child in root)
			{
				if (child.TryGetComponent(out Resolver _)) continue;

				foreach (var component in GetAffectedComponents(child, depth + 1))
				{
					yield return component;
				}
			}
		}

		public static IEnumerable<MethodInfo> GetMethodsIn(MonoBehaviour component) =>
			component.GetType()
				.GetMethods()
				.Where(IsSuitableMethod);

		private static bool IsSuitableMethod(MethodInfo method) =>
			method.Name == Constructor &&
			method.IsPublic && method.ReturnType == typeof(void);

		public const string Constructor = "Construct";
	}
}