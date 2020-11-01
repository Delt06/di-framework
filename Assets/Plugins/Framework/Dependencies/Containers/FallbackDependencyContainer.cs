using System;
using Framework.Core.Shared;
using UnityEngine;
using static Framework.Dependencies.Containers.DependencyExceptionFactory;
using Object = UnityEngine.Object;

namespace Framework.Dependencies.Containers
{
	public sealed class FallbackDependencyContainer : MonoBehaviour, IDependencyContainer
	{ 
		public void EnsureInitialized() { }

		public bool TryResolve<T>(out T dependency) where T : class
		{
			if (TryResolve(typeof(T), out var foundDependency))
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
			if (!typeof(Object).IsAssignableFrom(type))
			{
				dependency = default;
				return false;
			} 

			if (_cache.TryGet(type, out dependency))
				return true;

			dependency = FindObjectOfType(type);
			if (dependency == null) return false;
			
			_cache.TryRegister(dependency, out _);
			return true;

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
		
		private readonly TypedCache _cache = new TypedCache();
	}
}