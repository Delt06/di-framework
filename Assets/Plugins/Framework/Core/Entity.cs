using System;
using Framework.Core.Shared;
using Framework.Dependencies;
using UnityEngine;

namespace Framework.Core
{
	[DisallowMultipleComponent]
	public sealed class Entity : MonoBehaviour, IEntity
	{
		public bool TryFindComponent(Type type, out object component)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			return _components.TryGet(type, out component);
		}

		public bool TryFindComponent<T>(out T component) where T : class
		{
			return _components.TryGet(out component);
		}

		public T RequireComponent<T>() where T : class
		{
			if (TryFindComponent(out T component))
				return component;
			
			throw new ArgumentException($"Entity {this} does not have a component of type {typeof(T)}.");
		}

		public T ResolveGlobal<T>() where T : class
		{
			EnsureDependencyContainerExists();
			return _dependencyContainer.Resolve<T>();
		}

		private void EnsureDependencyContainerExists()
		{
			if (!_dependencyContainer)
				_dependencyContainer = FindObjectOfType<RootDependencyContainer>();

			if (!_dependencyContainer)
				throw new InvalidOperationException("Dependency container does not exist.");
		}

		public object ResolveGlobal(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			EnsureDependencyContainerExists();
			return _dependencyContainer.Resolve(type);
		}

		private RootDependencyContainer _dependencyContainer;
		private readonly TypedCache _components = new TypedCache();
	}
}