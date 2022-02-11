using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DELTation.DIFramework.Containers
{
    /// <summary>
    ///     Container that defines its object with a list.
    /// </summary>
    [AddComponentMenu("Dependency Container/List Dependency Container")]
    public sealed class ListDependencyContainer : DependencyContainerBase
    {
        [SerializeField] private List<Object> _dependencies = new List<Object>();

        public void Add([NotNull] Object @object)
        {
            if (@object == null) throw new ArgumentNullException(nameof(@object));
            _dependencies.Add(@object);
        }

        protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
        {
            for (var index = 0; index < _dependencies.Count; index++)
            {
                var dependency = _dependencies[index];

                if (dependency == null)
                {
                    Debug.LogError($"Dependency at index {index} is null.", this);
                    continue;
                }

                if (dependency is GameObject)
                {
                    Debug.LogWarning($"Dependency at index {index} is a GameObject ({dependency}), it will be ignored.",
                        this
                    );
                    continue;
                }

                builder.Register(dependency);
            }
        }
    }
}