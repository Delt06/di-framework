using System;
using System.Reflection;
using UnityEngine;

namespace DELTation.DIFramework
{
	[DefaultExecutionOrder(-100), DisallowMultipleComponent]
	public sealed class Resolver : MonoBehaviour, IInitializable
	{
		[SerializeField] private DependencySource _dependencySource = DependencySources.All;
		[SerializeField] private bool _destroyWhenFinished = true;

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
				Inject(component);
			}

			_cache.Clear();

			if (_destroyWhenFinished)
				Destroy(this);
		}

		private void Inject(MonoBehaviour component)
		{
			foreach (var method in Resolution.GetMethodsIn(component))
			{
				InjectThrough(component, method);
			}
		}

		private void InjectThrough(MonoBehaviour component, MethodInfo method)
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
			if (_cache.TryGet(type, out var dependency)) return dependency;

			var context = new Context(this, component);
			if (_dependencySource.TryResolve(context, type, out dependency))
			{
				_cache.TryRegister(dependency, out _);
				return dependency;
			}

			throw Exception($"Did not resolve dependency of type {type}.");
		}

		public bool CabBeResolvedSafe(MonoBehaviour component, Type type)
		{
			var context = new Context(this, component);
			return _dependencySource.CanBeResolvedSafe(context, type);
		}

		private static Exception Exception(string message) => new ArgumentException(message);

		void IInitializable.EnsureInitialized() => Resolve();

		private readonly TypedCache _cache = new TypedCache();
		private bool _resolved;
	}
}