using System;
using Framework.Core.Shared;
using JetBrains.Annotations;
using UnityEngine;
using static Framework.Dependencies.Containers.DependencyExceptionFactory;

namespace Framework.Dependencies.Containers
{
	public abstract class DependencyContainerBase : MonoBehaviour, IDependencyContainer
	{
		public void EnsureInitialized()
		{
			if (_initialized) return;

			ComposeDependencies();

			_initialized = true;
		}

		protected abstract void ComposeDependencies();

		protected void Register([NotNull] object dependency)
		{
			if (dependency == null) throw new ArgumentNullException(nameof(dependency));

			if (_cache.TryRegister(dependency, out var registeredDependency))
				return;

			var type = dependency.GetType();
			throw AlreadyRegistered(type, registeredDependency);
		}

		public bool TryResolve<T>(out T dependency) where T : class => _cache.TryGet(out dependency);

		public bool TryResolve(Type type, out object dependency)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			return _cache.TryGet(type, out dependency);
		}

		public T Resolve<T>() where T : class
		{
			if (TryResolve(out T dependency))
				return dependency;

			throw NotRegistered(typeof(T));
		}

		public object Resolve(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));

			if (TryResolve(type, out var dependency))
				return dependency;

			throw NotRegistered(type);
		}

		private readonly TypedCache _cache = new TypedCache();
		private bool _initialized;
	}
}