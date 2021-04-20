using DELTation.DIFramework.Containers;
using NUnit.Framework;
using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime
{
    public class DiTests
    {
        [Test]
        public void GivenContainerWithRegisteredObject_WhenTryingToResolveGlobally_ThenReturnsTheRegisteredObjectIs()
        {
            // Arrange
            var dependency = new GameObject().AddComponent<Rigidbody>();

            var gameObject = new GameObject();
            gameObject.AddComponent<ListDependencyContainer>().Add(dependency);
            gameObject.AddComponent<RootDependencyContainer>();

            // Act
            var resolved = Di.TryResolveGlobally<Rigidbody>(out var resolvedDependency);

            // Assert
            Assert.That(resolved);
            Assert.That(resolvedDependency, Is.EqualTo(dependency));
        }

        [Test]
        public void GivenContainerWithRegisteredObject_WhenTryingToResolveGloballyOtherType_ThenReturnsFalse()
        {
            // Arrange
            var dependency = new GameObject().AddComponent<Rigidbody>();

            var gameObject = new GameObject();
            gameObject.AddComponent<ListDependencyContainer>().Add(dependency);
            gameObject.AddComponent<RootDependencyContainer>();

            // Act
            var resolved = Di.TryResolveGlobally<BoxCollider>(out _);

            // Assert
            Assert.That(resolved, Is.False);
        }

        [Test]
        public void GivenDi_WhenTryingToResolveGloballyNullType_ThenThrowsArgumentNullException()
        {
            // Arrange

            // Act

            // Assert
            Assert.That(() => Di.TryResolveGlobally(null, out _), Throws.ArgumentNullException);
        }

        [Test]
        public void GivenContainerWithRegisteredObject_WhenCheckingWhetherCanResolveGloballySafe_ThenReturnsTrue()
        {
            // Arrange
            var dependency = new GameObject().AddComponent<Rigidbody>();

            var gameObject = new GameObject();
            gameObject.AddComponent<ListDependencyContainer>().Add(dependency);
            gameObject.AddComponent<RootDependencyContainer>();

            // Act
            var resolved = Di.CanBeResolvedGloballySafe<Rigidbody>();

            // Assert
            Assert.That(resolved);
        }

        [Test]
        public void
            GivenContainerWithRegisteredObject_WhenCheckingWhetherCanResolveGloballySafeOtherType_ThenReturnsFalse()
        {
            // Arrange
            var dependency = new GameObject().AddComponent<Rigidbody>();

            var gameObject = new GameObject();
            gameObject.AddComponent<ListDependencyContainer>().Add(dependency);
            gameObject.AddComponent<RootDependencyContainer>();

            // Act
            var resolved = Di.CanBeResolvedGloballySafe<BoxCollider>();

            // Assert
            Assert.That(resolved, Is.False);
        }

        [Test]
        public void GivenDi_WhenCheckingWhetherCanResolveGloballySafeNullType_ThenThrowsArgumentNullException()
        {
            // Arrange

            // Act

            // Assert
            Assert.That(() => Di.CanBeResolvedGloballySafe(null), Throws.ArgumentNullException);
        }
    }
}