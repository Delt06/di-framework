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

        internal readonly List<IDestroyable> Destroyables = new List<IDestroyable>();
        internal readonly List<IFixedUpdatable> FixedUpdatables = new List<IFixedUpdatable>();
        internal readonly List<ILateUpdatable> LateUpdatables = new List<ILateUpdatable>();
        internal readonly List<IStartable> Startables = new List<IStartable>();
        internal readonly List<IUpdatable> Updatables = new List<IUpdatable>();

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
            for (var index = 0; index < Startables.Count; index++)
            {
                Startables[index].OnStart();
            }
        }

        private void InvokeUpdatables()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < Updatables.Count; index++)
            {
                Updatables[index].OnUpdate();
            }
        }

        private void InvokeFixedUpdatables()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < FixedUpdatables.Count; index++)
            {
                FixedUpdatables[index].OnFixedUpdate();
            }
        }

        protected virtual void InvokeLateUpdatables()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < LateUpdatables.Count; index++)
            {
                LateUpdatables[index].OnLateUpdate();
            }
        }

        private void InvokeDestroyables()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < Destroyables.Count; index++)
            {
                Destroyables[index].OnDestroy();
            }
        }

        private void PullObjectsFromContainer()
        {
            var allObjects = new List<object>();
            _container.GetAllRegisteredExternalObjects(allObjects);

            for (var index = 0; index < allObjects.Count; index++)
            {
                var obj = allObjects[index];
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (obj is IStartable startable)
                    Startables.Add(startable);
                if (obj is IUpdatable updatable)
                    Updatables.Add(updatable);
                if (obj is IFixedUpdatable fixedUpdatable)
                    FixedUpdatables.Add(fixedUpdatable);
                if (obj is ILateUpdatable lateUpdatable)
                    LateUpdatables.Add(lateUpdatable);
                if (obj is IDestroyable destroyable)
                    Destroyables.Add(destroyable);
            }
        }
    }
}