using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Framework.Core
{
	public interface IEntity
	{
		GameObject gameObject { get; }

		bool TryFindComponent([NotNull] Type type, out object component);
		bool TryFindComponent<T>(out T component) where T : class;

		T RequireComponent<T>() where T : class;

		T ResolveGlobal<T>() where T : class;
		object ResolveGlobal([NotNull] Type type);
	}
}