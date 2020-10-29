using System;
using ECS.Core.Shared;
using JetBrains.Annotations;
using UnityEngine;

namespace ECS.Core
{
	public interface IEntity : IInitializable
	{
		GameObject gameObject { get; }

		bool TryFindComponent([NotNull] Type type, out object component);
		bool TryFindComponent<T>(out T component) where T : class, IComponent;
		T ResolveGlobal<T>() where T : class;
		object ResolveGlobal([NotNull] Type type);
	}
}