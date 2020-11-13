using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.Dependencies
{
	[AddComponentMenu("Dependency Container/Root Dependency Container")]
	public sealed class RootDependencyContainer : MonoBehaviour, IDependencyContainer
	{
		internal static RootDependencyContainer Instance =>
			_instance ? _instance : _instance = FindObjectOfType<RootDependencyContainer>();

		private static RootDependencyContainer _instance;

		public void EnsureInitialized()
		{
			if (_initialized) return;

			Initialize();
			_initialized = true;
		}

		private void Initialize()
		{
			_subContainers = GetChildContainers().ToArray();

			foreach (var subContainer in _subContainers)
			{
				subContainer.EnsureInitialized();
			}
		}

		private IEnumerable<IDependencyContainer> GetChildContainers() =>
			GetComponentsInChildren<IDependencyContainer>()
				.Where(c => !ReferenceEquals(c, this));

		public bool CanBeResolvedSafe(Type type) => GetChildContainers().Any(c => c.CanBeResolvedSafe(type));

		public bool TryResolve(Type type, out object dependency)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			EnsureInitialized();

			if (_cache.TryGet(type, out dependency))
				return true;

			foreach (var subContainer in _subContainers)
			{
				subContainer.EnsureInitialized();
				if (!subContainer.TryResolve(type, out dependency)) continue;

				_cache.TryRegister(dependency, out _);
				EnsureInitialized(dependency);
				return true;
			}

			dependency = default;
			return false;
		}

		private static void EnsureInitialized(object dependency)
		{
			if (!(dependency is MonoBehaviour behaviour)) return;
			var initializable = behaviour.GetComponents<IInitializable>();

			foreach (var i in initializable)
			{
				i.EnsureInitialized();
			}
		}

		private bool _initialized;
		private IDependencyContainer[] _subContainers = Array.Empty<IDependencyContainer>();
		private readonly TypedCache _cache = new TypedCache();
	}
}