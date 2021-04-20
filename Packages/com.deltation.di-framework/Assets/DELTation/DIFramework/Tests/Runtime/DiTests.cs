using System;
using DELTation.DIFramework.Containers;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

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
    }
}