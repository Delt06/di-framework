using System;
using ECS.Components;
using ECS.Core;
using ECS.Core.Shared;
using ECS.Dependencies;
using UnityEngine;

namespace ECS.Entities
{
	[DisallowMultipleComponent]
	public sealed class Entity : MonoBehaviour, IEntity
	{
		[SerializeField] private bool _locateDisabledComponents = false;

		public bool TryFindComponent(Type type, out object component)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			return _components.TryGet(type, out component);
		}

		public bool TryFindComponent<T>(out T component) where T : class, IComponent
		{
			return _components.TryGet(out component);
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

		private void OnEnable()
		{
			EntityWorld.Add(this);
		}

		private void OnDisable()
		{
			EntityWorld.Remove(this);
		}

		public void EnsureInitialized()
		{
			if (_initialized) return;

			foreach (var component in GetAllComponents())
			{
				if (_components.TryRegister(component, out var existingObject))
				{
					component.EnsureInitialized();
				}
				else
				{
					var type = component.GetType();
					Debug.LogError($"Can't cache {component} as {type}: {existingObject} is already cached.");
				}
			}

			_initialized = true;
		}

		private ComponentBase[] GetAllComponents()
		{
			return GetComponentsInChildren<ComponentBase>(_locateDisabledComponents);
		}

		private RootDependencyContainer _dependencyContainer;
		private readonly TypedCache _components = new TypedCache();
		private bool _initialized;
	}
}