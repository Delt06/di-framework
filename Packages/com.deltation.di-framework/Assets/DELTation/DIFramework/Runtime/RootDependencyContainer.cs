using System;
using System.Collections.Generic;
using DELTation.DIFramework.Editor;
using DELTation.DIFramework.Pooling;
using UnityEngine;

namespace DELTation.DIFramework
{
    [AddComponentMenu("Dependency Container/Root Dependency Container"), DisallowMultipleComponent]
    public sealed class RootDependencyContainer : MonoBehaviour, IDependencyContainer, IShowIconInHierarchy
    {
        [SerializeField] private bool _dontDestroyOnLoad = false;

        internal static RootDependencyContainer GetInstance(int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), index, "Index cannot be negative.");
            if (index >= InstancesCount)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"Index cannot be greater or equal to InstancesCount ({InstancesCount})."
                );

            return Containers[index];
        }

        internal static int InstancesCount => Containers.Count;

        private static void Register(RootDependencyContainer container)
        {
            if (!Containers.Contains(container))
                Containers.Add(container);
        }

        private static void Unregister(RootDependencyContainer container) => Containers.Remove(container);

        private static readonly List<RootDependencyContainer> Containers = new List<RootDependencyContainer>();

        private void EnsureInitialized()
        {
            if (_initialized) return;

            Initialize();
            _initialized = true;
        }

        private void Initialize()
        {
            _subContainers = GetChildContainers();
        }

        private IDependencyContainer[] GetChildContainers()
        {
            var containersList = ListPool<IDependencyContainer>.Rent();
            GetComponentsInChildren(containersList);
            containersList.Remove(this);
            var containersArray = containersList.ToArray();
            ListPool<IDependencyContainer>.Return(containersList);
            return containersArray;
        }

        public bool CanBeResolvedSafe(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            foreach (var childContainer in GetChildContainers())
            {
                if (childContainer.CanBeResolvedSafe(type))
                    return true;
            }

            return false;
        }

        /// <inheritdoc />
        public void GetAllRegisteredObjects(HashSet<object> objects)
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            EnsureInitialized();

            foreach (var subContainer in _subContainers)
            {
                subContainer.GetAllRegisteredObjects(objects);
            }
        }

        public bool TryResolve(Type type, out object dependency)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            EnsureInitialized();

            if (_cache.TryGet(type, out dependency))
                return true;

            foreach (var subContainer in _subContainers)
            {
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

            var initializable = ListPool<IInitializable>.Rent();
            behaviour.GetComponents(initializable);

            for (var index = 0; index < initializable.Count; index++)
            {
                initializable[index].EnsureInitialized();
            }

            ListPool<IInitializable>.Return(initializable);
        }

        private void OnEnable()
        {
            Register(this);
        }

        private void OnDisable()
        {
            Unregister(this);
        }

        private void Awake()
        {
            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        private bool _initialized;
        private IDependencyContainer[] _subContainers = Array.Empty<IDependencyContainer>();
        private readonly TypedCache _cache = new TypedCache();
    }
}