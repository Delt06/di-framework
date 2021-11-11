using System.Collections;
using System.Collections.Generic;
using DELTation.DIFramework.Containers;
using DELTation.DIFramework.Lifecycle;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DELTation.DIFramework.Tests.Runtime
{
    public class ContainerLifecycleTests : TestFixtureBase
    {
        private DependencyContainer _container;
        private Destroyable _destroyable;
        private Startable _startable;
        private Updatable _updatable;

        [SetUp]
        public void SetUp()
        {
            _startable = new Startable();
            _updatable = new Updatable();
            _destroyable = new Destroyable();

            _container = CreateContainerWith<DependencyContainer>();
        }

        private void InitLifecycle()
        {
            var lifecycle = _container.gameObject.AddComponent<ContainerLifecycle>();
            lifecycle.Container = _container;
        }

        [UnityTest]
        public IEnumerator GivenContainerWithLifecycle_WhenEmpty_ThenNoCalls()
        {
            // Arrange
            InitLifecycle();

            // Act
            yield return null;

            // Assert
            AssertThatCallsCountsAreEqual(0, 0, 0);
        }

        [UnityTest]
        public IEnumerator GivenContainerWithLifecycle_WhenStarted_ThenStartIsCalled()
        {
            // Arrange
            AddAllObjects();
            InitLifecycle();

            // Act
            yield return null;

            // Assert
            AssertThatCallsCountsAreEqual(1, 1, 0);
        }

        [UnityTest]
        public IEnumerator GivenContainerWithLifecycle_WhenStartedAndExecutedSeveralFrames_ThenStartIsCalled()
        {
            // Arrange
            AddAllObjects();
            InitLifecycle();

            // Act
            yield return null;
            yield return null;

            // Assert
            AssertThatCallsCountsAreEqual(1, 2, 0);
        }

        [UnityTest]
        public IEnumerator GivenContainerWithLifecycle_WhenDestroyed_ThenStartIsCalled()
        {
            // Arrange
            AddAllObjects();
            InitLifecycle();

            // Act
            yield return null;
            Object.Destroy(_container.GetComponent<ContainerLifecycle>());
            yield return null;

            // Assert
            AssertThatCallsCountsAreEqual(1, 1, 1);
        }

        private void AddAllObjects()
        {
            _container.ObjectsToRegister.Add(_startable);
            _container.ObjectsToRegister.Add(_updatable);
            _container.ObjectsToRegister.Add(_destroyable);
        }

        private void AssertThatCallsCountsAreEqual(int startableCalls, int updatableCalls, int destroyableCalls)
        {
            Assert.AreEqual(startableCalls, _startable.CallsCount);
            Assert.AreEqual(updatableCalls, _updatable.CallsCount);
            Assert.AreEqual(destroyableCalls, _destroyable.CallsCount);
        }

        private class Startable : IStartable
        {
            public int CallsCount { get; private set; }

            public void OnStart()
            {
                CallsCount++;
            }
        }

        private class Updatable : IUpdatable
        {
            public int CallsCount { get; private set; }

            public void OnUpdate()
            {
                CallsCount++;
            }
        }

        private class Destroyable : IDestroyable
        {
            public int CallsCount { get; private set; }

            public void OnDestroy()
            {
                CallsCount++;
            }
        }

        private class DependencyContainer : DependencyContainerBase
        {
            public List<object> ObjectsToRegister { get; } = new List<object>();

            protected override void ComposeDependencies(ContainerBuilder builder)
            {
                foreach (var o in ObjectsToRegister)
                {
                    builder.Register(o);
                }
            }
        }
    }
}