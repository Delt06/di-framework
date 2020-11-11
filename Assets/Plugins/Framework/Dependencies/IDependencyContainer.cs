using System;
using Framework.Dependencies.Containers;
using JetBrains.Annotations;

namespace Framework.Dependencies
{
	public interface IDependencyContainer
	{
		bool TryResolve<T>(out T dependency) where T : class;
		bool TryResolve([NotNull] Type type, out object dependency);
		T Resolve<T>() where T : class;
		object Resolve([NotNull] Type type);
		void EnsureInitialized();
	}
}