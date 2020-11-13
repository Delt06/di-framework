using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Framework.Dependencies;
using UnityEngine;

namespace Framework
{
	[DefaultExecutionOrder(-100), DisallowMultipleComponent]
	public sealed class Resolver : MonoBehaviour, IInitializable
	{
		[SerializeField] private DependencySource _dependencySource = DependencySources.All;

		private void Awake()
		{
			Resolve();
		}

		private void Resolve()
		{
			if (_resolved) return;

			_resolved = true;

			foreach (var (component, _) in GetAffectedComponents(transform))
			{
				Process(component);
			}
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

		private void Process(MonoBehaviour component)
		{
			foreach (var method in GetResolutionMethods(component))
			{
				ProcessMethod(component, method);
			}
		}

		public static IEnumerable<MethodInfo> GetResolutionMethods(MonoBehaviour component) =>
			component.GetType()
				.GetMethods()
				.Where(IsSuitableMethod);

		private static bool IsSuitableMethod(MethodInfo method) =>
			method.Name == Constructor &&
			method.IsPublic && method.ReturnType == typeof(void);

		private void ProcessMethod(MonoBehaviour component, MethodInfo method)
		{
			var parameters = method.GetParameters();
			var arguments = new object[parameters.Length];

			for (var index = 0; index < parameters.Length; index++)
			{
				var parameter = parameters[index];
				VerifyParameter(parameter);
				arguments[index] = Resolve(component, parameter.ParameterType);
			}

			method.Invoke(component, arguments);
		}

		private static void VerifyParameter(ParameterInfo parameter)
		{
			var parameterType = parameter.ParameterType;
			if (parameterType.IsValueType)
				throw Exception("Injection methods cannot have value-type parameters.");
			if (parameter.IsOut)
				throw Exception("Injection methods cannot have out parameters.");
			if (parameter.IsIn)
				throw Exception("Injection methods cannot have in parameters.");
			if (parameterType.IsByRef)
				throw Exception("Injection methods cannot have ref parameters.");
		}

		private object Resolve(MonoBehaviour component, Type type)
		{
			var context = new Context(this, component);
			if (_dependencySource.TryResolve(context, type, out var dependency))
				return dependency;

			throw Exception($"Did not resolve dependency of type {type}.");
		}

		public bool CabBeResolvedSafe(MonoBehaviour component, Type type)
		{
			var context = new Context(this, component);
			return _dependencySource.CanBeResolvedSafe(context, type);
		}

		private static Exception Exception(string message) => new ArgumentException(message);

		void IInitializable.EnsureInitialized() => Resolve();

		private const string Constructor = "Construct";
		private bool _resolved;
	}
}