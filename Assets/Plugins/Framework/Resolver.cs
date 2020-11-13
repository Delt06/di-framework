using System;
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
			ProcessNodeRecursively(transform);
		}

		private void ProcessNodeRecursively(Transform root)
		{
			var components = root.GetComponents<MonoBehaviour>();

			foreach (var component in components)
			{
				Process(component);
			}

			foreach (Transform child in root)
			{
				if (child.TryGetComponent(out Resolver _)) continue;
				ProcessNodeRecursively(child);
			}
		}

		private void Process(MonoBehaviour component)
		{
			var type = component.GetType();
			var methods = type.GetMethods();

			foreach (var method in methods)
			{
				ProcessMethod(component, method);
			}
		}

		private void ProcessMethod(MonoBehaviour component, MethodInfo method)
		{
			if (method.Name != Constructor) return;
			if (!method.IsPublic) return;
			if (method.ReturnType != typeof(void)) return;

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

		private static Exception Exception(string message) => new ArgumentException(message);

		void IInitializable.EnsureInitialized() => Resolve();

		private const string Constructor = "Construct";
		private bool _resolved;
	}
}