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

		public bool TryFindComponent<T>(out T component) where T : class => _components.TryGet(out component);

		public T RequireComponent<T>() where T : class
		{
			if (TryFindComponent(out T component))
				return component;

			throw new ArgumentException($"Entity {this} does not have a component of type {typeof(T)}.");
		}

		public T ResolveGlobal<T>() where T : class => RootDependencyContainer.Instance.Resolve<T>();

		public object ResolveGlobal(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			return RootDependencyContainer.Instance.Resolve(type);
		}

		private readonly TypedCache _components = new TypedCache();
	}
}