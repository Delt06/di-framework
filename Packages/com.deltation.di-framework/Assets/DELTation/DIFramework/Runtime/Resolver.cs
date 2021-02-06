using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DELTation.DIFramework
{
	[DisallowMultipleComponent]
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

			_affectedComponents.Clear();
			Resolution.GetAffectedComponents(_affectedComponents, transform);
			
			foreach (var (component, _) in _affectedComponents)
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
			if (!Resolution.TryGetInjectableParameters(method, out var parameters)) 
				throw new InvalidOperationException($"{component}'s {Resolution.Constructor} method is not injectable.");

			var arguments = new object[parameters.Count];

			for (var index = 0; index < parameters.Count; index++)
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
			if (_dependencySource.TryResolve(context, type, out dependency, out var actualSource))
			{
				if (IsCacheable(actualSource))
					_cache.TryRegister(dependency, out _);

				return dependency;
			}

			throw Exception($"Did not resolve dependency of type {type}.");
		}

		private bool IsCacheable(DependencySource source) => source != DependencySource.Local;

		public bool CabBeResolvedSafe(MonoBehaviour component, Type type)
		{
			var context = new Context(this, component);
			return _dependencySource.CanBeResolvedSafe(context, type);
		}

		private static Exception Exception(string message) => new ArgumentException(message);

		void IInitializable.EnsureInitialized() => Resolve();

		private readonly TypedCache _cache = new TypedCache();
		private bool _resolved;
		private readonly List<(MonoBehaviour component, int depth)> _affectedComponents = new List<(MonoBehaviour, int depth)>();
	}
}