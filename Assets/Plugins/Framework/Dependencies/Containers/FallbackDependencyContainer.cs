using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Dependencies.Containers
{
	[DisallowMultipleComponent, AddComponentMenu("Dependency Container/Fallback Container")]
	public sealed class FallbackDependencyContainer : MonoBehaviour, IDependencyContainer
	{
		public void EnsureInitialized() { }

		public bool CanBeResolvedSafe(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			return TryFindObjectOfType(type, out _);
		}

		public bool TryResolve(Type type, out object dependency)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			if (_cache.TryGet(type, out dependency)) return true;
			if (!TryFindObjectOfType(type, out dependency)) return false;

			_cache.TryRegister(dependency, out _);
			return true;
		}

		private static bool TryFindObjectOfType(Type type, out object dependency)
		{
			if (!typeof(Object).IsAssignableFrom(type))
			{
				dependency = default;
				return false;
			}

			var dependencies = FindObjectsOfType(type);

			foreach (var d in dependencies)
			{
				if (d.ShouldBeIgnoredByContainer()) continue;

				dependency = d;
				return true;
			}

			dependency = default;
			return false;
		}

		private readonly TypedCache _cache = new TypedCache();
	}
}