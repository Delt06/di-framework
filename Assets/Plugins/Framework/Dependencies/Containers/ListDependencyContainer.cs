using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Dependencies.Containers
{
	[AddComponentMenu("Dependency Container/List Dependency Container")]
	public sealed class ListDependencyContainer : DependencyContainerBase
	{
		[SerializeField] private List<Object> _dependencies = new List<Object>();

		public void Add(Object @object)
		{
			if (_frozen) throw new InvalidOperationException("Container is frozen.");
			_dependencies.Add(@object);
		}

		protected override void ComposeDependencies()
		{
			for (var index = 0; index < _dependencies.Count; index++)
			{
				if (_dependencies[index] == null)
				{
					Debug.LogError($"Dependency at index {index} is null.", this);
					continue;
				}

				var dependency = _dependencies[index];
				Register(dependency);
			}

			_frozen = true;
		}

		private bool _frozen;
	}
}