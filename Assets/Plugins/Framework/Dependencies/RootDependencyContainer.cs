using System;
using System.Linq;
using Framework.Core.Shared;
using UnityEngine;
using static Framework.Dependencies.Containers.DependencyExceptionFactory;

namespace Framework.Dependencies
{
	[AddComponentMenu("Dependency Container/Root Dependency Container")]
	public sealed class RootDependencyContainer : MonoBehaviour, IDependencyContainer
	{
		public static RootDependencyContainer Instance =>
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
			_subContainers = GetComponentsInChildren<IDependencyContainer>()
				.Where(c => !ReferenceEquals(c, this))
				.ToArray();

			foreach (var subContainer in _subContainers)
			{
				subContainer.EnsureInitialized();
			}
		}

		public bool TryResolve<T>(out T dependency) where T : class
		{
			var type = typeof(T);

			if (TryResolve(type, out var foundDependency))
			{
				dependency = (T) foundDependency;
				return true;
			}

			dependency = default;
			return false;
		}

		public bool TryResolve(Type type, out object dependency)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			EnsureInitialized();

			foreach (var subContainer in _subContainers)
			{
				subContainer.EnsureInitialized();
				if (!subContainer.TryResolve(type, out dependency)) continue;

				if (dependency is IInitializable initializable)
					initializable.EnsureInitialized();

				return true;
			}

			dependency = default;
			return false;
		}

		public T Resolve<T>() where T : class
		{
			if (TryResolve(out T dependency))
				return dependency;

			throw NotRegistered(typeof(T));
		}

		public object Resolve(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));

			if (TryResolve(type, out var dependency))
				return dependency;

			throw NotRegistered(type);
		}

		private void Awake()
		{
			if (Instance != this)
			{
				Debug.LogWarning("A duplicate container was found and destroyed.", gameObject);
				Destroy(this);
			}
		}

		private bool _initialized;
		private IDependencyContainer[] _subContainers = Array.Empty<IDependencyContainer>();
	}
}