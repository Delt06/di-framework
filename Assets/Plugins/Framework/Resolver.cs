using System;
using System.Reflection;
using Framework.Dependencies;
using UnityEngine;

namespace Framework
{
	[DefaultExecutionOrder(-100)]
	public sealed class Resolver : MonoBehaviour, IInitializable
	{
		[SerializeField] private DependencySource _dependencySource = DependencySources.All;

		private void Awake()
		{
			Resolve();
		}

		public void Resolve()
		{
			if (_resolved) return;

			_resolved = true;
			var children = GetComponentsInChildren<MonoBehaviour>(true);

			foreach (var child in children)
			{
				ProcessChild(child);
			}
		}

		private void ProcessChild(MonoBehaviour child)
		{
			var type = child.GetType();
			var methods = type.GetMethods();

			foreach (var method in methods)
			{
				ProcessMethod(child, method);
			}
		}

		private void ProcessMethod(MonoBehaviour child, MethodInfo method)
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
				arguments[index] = Resolve(child, parameter.ParameterType);
			}

			method.Invoke(child, arguments);
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

		private object Resolve(MonoBehaviour child, Type type)
		{
			var context = new Context(this, child);
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