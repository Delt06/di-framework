using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using static Framework.Dependencies.DependencyExceptionFactory;

namespace Framework.Dependencies.Containers
{
	public abstract class DependencyContainerBase : MonoBehaviour, IDependencyContainer
	{
		public void EnsureInitialized()
		{
			if (_initialized) return;

			var builder = new ContainerBuilder();
			ComposeDependencies(builder);

			for (var index = 0; index < builder.DependenciesCount; index++)
			{
				if (builder.TryGetObject(index, out var obj))
					Register(obj);
				else if (builder.TryGetType(index, out var type))
					Register(Activator.CreateInstance(type));
				else
					throw new InvalidOperationException("Neither object nor type is present in the builder.");
			}

			_initialized = true;
		}

		protected abstract void ComposeDependencies(ContainerBuilder builder);

		public bool CanBeResolvedSafe(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));

			var builder = new ContainerBuilder();
			ComposeDependencies(builder);
			return Enumerable.Range(0, builder.DependenciesCount)
				.Any(i => ConformsTo(builder, i, type));
		}

		private static bool ConformsTo(ContainerBuilder builder, int index, Type checkedType) =>
			builder.TryGetObject(index, out var @object) && checkedType.IsInstanceOfType(@object) ||
			builder.TryGetType(index, out var type) && checkedType.IsAssignableFrom(type);

		private void Register([NotNull] object dependency)
		{
			if (dependency == null) throw new ArgumentNullException(nameof(dependency));
			if (dependency.ShouldBeIgnoredByContainer()) return;

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