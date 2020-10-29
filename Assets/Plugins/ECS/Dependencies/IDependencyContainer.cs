using System;
using ECS.Core.Shared;
using JetBrains.Annotations;

namespace ECS.Dependencies
{
	public interface IDependencyContainer : IInitializable
	{
		bool TryResolve<T>(out T dependency) where T : class;
		bool TryResolve([NotNull] Type type, out object dependency);
		T Resolve<T>() where T : class;
		object Resolve([NotNull] Type type);
	}
}