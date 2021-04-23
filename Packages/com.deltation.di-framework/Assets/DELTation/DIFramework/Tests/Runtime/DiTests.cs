using System;
using System.Collections.Generic;
using System.Linq;
using DELTation.DIFramework.Containers;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DELTation.DIFramework.Tests.Runtime
{
    public class DiTests : TestFixtureBase
    {
        [Test]
        public void GivenContainerWithRegisteredObject_WhenTryingToResolveGlobally_ThenReturnsTheRegisteredObjectIs()
        {
            // Arrange
            var dependency = NewGameObject().AddComponent<Rigidbody>();

            var gameObject = NewGameObject();
            gameObject.AddComponent<ListDependencyContainer>().Add(dependency);
            gameObject.AddComponent<RootDependencyContainer>();

            // Act
            var resolved = Di.TryResolveGlobally<Rigidbody>(out var resolvedDependency);

            // Assert
            resolved.Should().BeTrue();
            resolvedDependency.Should().Be(dependency);
        }

        [Test]
        public void GivenContainerWithRegisteredObject_WhenTryingToResolveGloballyOtherType_ThenReturnsFalse()
        {
            // Arrange
            var dependency = NewGameObject().AddComponent<Rigidbody>();

            var gameObject = NewGameObject();
            gameObject.AddComponent<ListDependencyContainer>().Add(dependency);
            gameObject.AddComponent<RootDependencyContainer>();

            // Act
            var resolved = Di.TryResolveGlobally<BoxCollider>(out _);

            // Assert
            resolved.Should().BeFalse();
        }

        [Test]
        public void GivenDi_WhenTryingToResolveGloballyNullType_ThenThrowsArgumentNullException()
        {
            // Arrange
            Action action = () => Di.TryResolveGlobally(null, out _);

            // Act

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void GivenContainerWithRegisteredObject_WhenCheckingWhetherCanResolveGloballySafe_ThenReturnsTrue()
        {
            // Arrange
            var dependency = NewGameObject().AddComponent<Rigidbody>();

            var gameObject = NewGameObject();
            gameObject.AddComponent<ListDependencyContainer>().Add(dependency);
            gameObject.AddComponent<RootDependencyContainer>();

            // Act
            var resolved = Di.CanBeResolvedGloballySafe<Rigidbody>();

            // Assert
            resolved.Should().BeTrue();
        }

        [Test]
        public void
            GivenContainerWithRegisteredObject_WhenCheckingWhetherCanResolveGloballySafeOtherType_ThenReturnsFalse()
        {
            // Arrange
            var dependency = NewGameObject().AddComponent<Rigidbody>();

            var gameObject = NewGameObject();
            gameObject.AddComponent<ListDependencyContainer>().Add(dependency);
            gameObject.AddComponent<RootDependencyContainer>();

            // Act
            var resolved = Di.CanBeResolvedGloballySafe<BoxCollider>();

            // Assert
            resolved.Should().BeFalse();
        }

        [Test]
        public void GivenDi_WhenCheckingWhetherCanResolveGloballySafeNullType_ThenThrowsArgumentNullException()
        {
            // Arrange
            Action action = () => Di.CanBeResolvedGloballySafe(null);

            // Act

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void GivenDi_WhenGettingAllRegisteredObjects_ThenItIsNotNull()
        {
            // Arrange

            // Act
            var allRegisteredObjects = Di.GetAllRegisteredObjects();

            // Assert
            allRegisteredObjects.Should().NotBeNull();
        }

        [Test]
        public void GivenDiWithNoRootContainers_WhenGettingAllRegisteredObjects_ThenItIsEmpty()
        {
            // Arrange

            // Act
            var allRegisteredObjects = Di.GetAllRegisteredObjects();

            // Assert
            allRegisteredObjects.Should().BeEmpty();
        }

        [Test]
        public void GivenDiWithOneRootContainer_WhenGettingAllRegisteredObjects_ThenReturnsAllObjectsFromContainer()
        {
            // Arrange
            var objects = new List<Object>
            {
                NewGameObject().AddComponent<Rigidbody>(),
                NewGameObject().AddComponent<SphereCollider>(),
                NewGameObject().AddComponent<BoxCollider>(),
            };

            var listDependencyContainer = CreateContainerWith<ListDependencyContainer>();
            objects.ForEach(o => listDependencyContainer.Add(o));

            // Act
            var allRegisteredObjects = Di.GetAllRegisteredObjects();

            // Assert
            allRegisteredObjects.Should().BeEquivalentTo(objects);
        }

        [Test]
        public void
            GivenDiWithOneRootContainerAndTwoSubContainers_WhenGettingAllRegisteredObjects_ThenReturnsAllObjectsFromBothSubContainers()
        {
            // Arrange
            var objects1 = new List<Object>
            {
                NewGameObject().AddComponent<Rigidbody>(),
                NewGameObject().AddComponent<SphereCollider>(),
                NewGameObject().AddComponent<BoxCollider>(),
            };

            var objects2 = new List<Object>
            {
                NewGameObject().AddComponent<BoxCollider2D>(),
                NewGameObject().AddComponent<CharacterController>(),
            };

            var listDependencyContainer1 = CreateContainerWith<ListDependencyContainer>();
            var listDependencyContainer2 = listDependencyContainer1.gameObject.AddComponent<ListDependencyContainer>();
            objects1.ForEach(o => listDependencyContainer1.Add(o));
            objects2.ForEach(o => listDependencyContainer2.Add(o));

            // Act
            var allRegisteredObjects = Di.GetAllRegisteredObjects();

            // Assert
            var allObjects = objects1.Concat(objects2).Distinct().ToArray();
            allRegisteredObjects.Should().BeEquivalentTo(allObjects);
        }

        [Test]
        public void
            GivenDiWithTwoRootContainers_WhenGettingAllRegisteredObjects_ThenReturnsAllObjectsFromBothRootContainers()
        {
            // Arrange
            var objects1 = new List<Object>
            {
                NewGameObject().AddComponent<Rigidbody>(),
                NewGameObject().AddComponent<SphereCollider>(),
                NewGameObject().AddComponent<BoxCollider>(),
            };

            var objects2 = new List<Object>
            {
                NewGameObject().AddComponent<BoxCollider2D>(),
                NewGameObject().AddComponent<CharacterController>(),
            };

            var listDependencyContainer1 = CreateContainerWith<ListDependencyContainer>();
            var listDependencyContainer2 = CreateContainerWith<ListDependencyContainer>();
            objects1.ForEach(o => listDependencyContainer1.Add(o));
            objects2.ForEach(o => listDependencyContainer2.Add(o));

            // Act
            var allRegisteredObjects = Di.GetAllRegisteredObjects();

            // Assert
            var allObjects = objects1.Concat(objects2).Distinct().ToArray();
            allRegisteredObjects.Should().BeEquivalentTo(allObjects);
        }
    }
}