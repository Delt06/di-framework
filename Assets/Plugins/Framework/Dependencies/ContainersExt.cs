using Framework.Dependencies.Containers;
using UnityEngine;

namespace Framework.Dependencies
{
	internal static class ContainersExt
	{
		public static bool ShouldBeIgnoredByContainer(this object obj) =>
			obj is IIgnoreByContainer || obj is IDependencyContainer ||
			obj is Component c && c.TryGetComponent(out IIgnoreByContainer _);
	}
}