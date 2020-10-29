using ECS.Core;
using ECS.Dependencies;
using UnityEngine;

namespace ECS.Components
{
	public abstract class ComponentBase : MonoBehaviour, IComponent
	{
		public void EnsureInitialized()
		{
			if (_initialized) return;
			
			Initialize();
			_initialized = true;
		}

		protected virtual void Initialize()
		{
			this.ResolveDependencies();
		}

		private bool _initialized;
	}
}