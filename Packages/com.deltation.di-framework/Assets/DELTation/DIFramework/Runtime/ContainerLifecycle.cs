using System;
using System.Collections.Generic;
using DELTation.DIFramework.Containers;
using DELTation.DIFramework.Lifecycle;
using UnityEngine;
using UnityEngine.Assertions;

namespace DELTation.DIFramework
{
    /// <summary>
    ///     Pulls all registered objects from a container and calls lifecycle events on them
    ///     (<see cref="DIFramework.Lifecycle.IStartable" />, <see cref="DIFramework.Lifecycle.IUpdatable" />,
    ///     <see cref="DIFramework.Lifecycle.IDestroyable" />).
    /// </summary>
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class ContainerLifecycle : MonoBehaviour
    {
        [SerializeField] private DependencyContainerBase _container;

        private readonly List<IDestroyable> _destroyables = new List<IDestroyable>();
        private readonly List<IFixedUpdatable> _fixedUpdatables = new List<IFixedUpdatable>();
        private readonly List<ILateUpdatable> _lateUpdatables = new List<ILateUpdatable>();
        private readonly List<IStartable> _startables = new List<IStartable>();
        private readonly List<IUpdatable> _updatables = new List<IUpdatable>();

        public DependencyContainerBase Container
        {
            get => _container;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value), "Container cannot be null.");
                _container = value;
            }
        }

        protected virtual void Start()
        {
            AssetThatContainerIsAssigned();
            PullObjectsFromContainer();
            InvokeStartables();
        }

        protected virtual void Update()
        {
            InvokeUpdatables();
        }

        protected virtual void FixedUpdate()
        {
            InvokeFixedUpdatables();
        }

        protected virtual void LateUpdate()
        {
            InvokeLateUpdatables();
        }

        protected virtual void OnDestroy()
        {
            InvokeDestroyables();
        }

        private void AssetThatContainerIsAssigned()
        {
            Assert.IsNotNull(_container, $"Container has to be assigned ({gameObject.name}).");
        }

        private void InvokeStartables()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < _startables.Count; index++)
            {
                _startables[index].OnStart();
            }
        }

        private void InvokeUpdatables()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < _updatables.Count; index++)
            {
                _updatables[index].OnUpdate();
            }
        }

        private void InvokeFixedUpdatables()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < _fixedUpdatables.Count; index++)
            {
                _fixedUpdatables[index].OnFixedUpdate();
            }
        }

        protected virtual void InvokeLateUpdatables()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < _lateUpdatables.Count; index++)
            {
                _lateUpdatables[index].OnLateUpdate();
            }
        }

        private void InvokeDestroyables()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < _destroyables.Count; index++)
            {
                _destroyables[index].OnDestroy();
            }
        }

        private void PullObjectsFromContainer()
        {
            var allObjects = new List<object>();
            _container.GetAllRegisteredObjects(allObjects);

            for (var index = 0; index < allObjects.Count; index++)
            {
                var obj = allObjects[index];
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (obj is IStartable startable)
                    _startables.Add(startable);
                if (obj is IUpdatable updatable)
                    _updatables.Add(updatable);
                if (obj is IFixedUpdatable fixedUpdatable)
                    _fixedUpdatables.Add(fixedUpdatable);
                if (obj is ILateUpdatable lateUpdatable)
                    _lateUpdatables.Add(lateUpdatable);
                if (obj is IDestroyable destroyable)
                    _destroyables.Add(destroyable);
            }
        }
    }
}