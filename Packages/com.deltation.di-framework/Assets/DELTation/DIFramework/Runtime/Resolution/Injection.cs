using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework.Resolution
{
	public static class Injection
	{
		public static void InvalidateCache()
		{
			InjectableParameters.Clear();
			InjectableMethods.Clear();
		}
		
		public static void WarmUp([NotNull] GameObject gameObject)
		{
			if (gameObject == null) throw new ArgumentNullException(nameof(gameObject));

			var components = gameObject.GetComponents<MonoBehaviour>();
			foreach (var component in components)
			{
				WarmUp(component.GetType());
			}
		}
		
		public static void WarmUp(params Type[] types)
		{
			foreach (var type in types)
			{
				WarmUp(type);
			}
		}
		
		public static void WarmUp([NotNull] Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			
			foreach (var methodInfo in GetSuitableMethodsIn(type))
			{
				TryGetInjectableParameters(methodInfo, out _);
			}
		}
		
		public static IEnumerable<Type> GetAllDependenciesOf([NotNull] Type componentType)
		{
			if (componentType == null) throw new ArgumentNullException(nameof(componentType));
			return GetSuitableMethodsIn(componentType)
				.SelectMany(m => m.GetParameters())
				.Select(p => p.ParameterType)
				.Distinct();
		}

		public static bool IsInjectable([NotNull] Type componentType)
		{
			if (componentType == null) throw new ArgumentNullException(nameof(componentType));
			var methods = GetSuitableMethodsIn(componentType);

			foreach (var method in methods)
			{
				if (!TryGetInjectableParameters(method, out _))
					return false;
			}

			return true;
		}

		internal static bool AreInjectable([NotNull] this ParameterInfo[] parameters)
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

		internal static void GetAffectedComponents(List<(MonoBehaviour component, int depth)> affectedComponents,
			Transform root,
			int depth = 0)
		{
			var components = root.GetComponents<MonoBehaviour>();

			foreach (var component in components)
			{
				if (component is Resolver) continue;
				affectedComponents.Add((component, depth));
			}

			foreach (Transform child in root)
			{
				if (child.TryGetComponent(out Resolver _)) continue;

				GetAffectedComponents(affectedComponents, child, depth + 1);
			}
		}

		public static IEnumerable<(MonoBehaviour component, int depth)> GetAffectedComponents(Transform root,
			int depth = 0)
		{
			var components = new List<(MonoBehaviour, int)>();
			GetAffectedComponents(components, root, depth);
			return components;
		}

		internal static IEnumerable<MethodInfo> GetSuitableMethodsIn(Type type)
		{
			if (InjectableMethods.TryGetValue(type, out var methods)) return methods;

			return InjectableMethods[type] = type
				.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(IsSuitableMethod)
				.ToArray();
		}

		internal static bool TryGetInjectableParameters(MethodInfo method, out IReadOnlyList<ParameterInfo> parameters)
		{
			if (InjectableParameters.TryGetValue(method, out var parametersInfo))
			{
				parameters = parametersInfo;
				return true;
			}

			parametersInfo = method.GetParameters();
			if (parametersInfo.AreInjectable())
			{
				InjectableParameters[method] = parametersInfo;
				parameters = parametersInfo;
				return true;
			}

			parameters = default;
			return false;
		}

		private static bool IsSuitableMethod(MethodInfo method) =>
			method.Name == Constructor &&
			method.IsPublic && method.ReturnType == typeof(void);

		public const string Constructor = "Construct";

		private static readonly IDictionary<Type, MethodInfo[]>
			InjectableMethods = new Dictionary<Type, MethodInfo[]>();

		private static readonly IDictionary<MethodInfo, ParameterInfo[]> InjectableParameters =
			new Dictionary<MethodInfo, ParameterInfo[]>();
	}
}