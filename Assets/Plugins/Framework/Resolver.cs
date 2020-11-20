using System;
using System.Reflection;
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

			foreach (var (component, _) in Resolution.GetAffectedComponents(transform))
			{
				Process(component);
			}
		}

		private void Process(MonoBehaviour component)
		{
			foreach (var method in Resolution.GetMethodsIn(component))
			{
				ProcessMethod(component, method);
			}
		}

		private void ProcessMethod(MonoBehaviour component, MethodInfo method)
		{
			var parameters = method.GetParameters();
			if (!parameters.AreInjectable())
				throw new InvalidOperationException(
					$"{component}'s {Resolution.Constructor} method is not injectable.");

			var arguments = new object[parameters.Length];

			for (var index = 0; index < parameters.Length; index++)
			{
				var parameter = parameters[index];
				arguments[index] = Resolve(component, parameter.ParameterType);
			}

			method.Invoke(component, arguments);
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

		private bool _resolved;
	}
}