using DELTation.DIFramework.Containers;
using NUnit.Framework;
using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime
{
    public class ContainerBuilderExtensionsTests : TestFixtureBase
    {
        [Test]
        public void GivenRegisterIfNotNull_WhenNull_ThenNoExceptions()
        {
            // Arrange
            CreateContainerWith<ContainerRegisteringNull>();

            // Act
            var resolved = Di.TryResolveGlobally<object>(out _);

            // Assert
            Assert.IsFalse(resolved);
        }

        [Test]
        public void GivenRegisterIfNotNull_WhenUnityNull_ThenNoExceptions()
        {
            // Arrange
            CreateContainerWith<ContainerRegisteringUnityNull>();

            // Act
            var resolved = Di.TryResolveGlobally<Collider>(out _);

            // Assert
            Assert.IsFalse(resolved);
        }

        [Test]
        public void GivenRegisterIfNotNull_WhenNotNull_ThenRegistered()
        {
            // Arrange
            CreateContainerWith<ContainerRegisteringNotNull>();

            // Act
            var resolved = Di.TryResolveGlobally<object>(out _);

            // Assert
            Assert.IsTrue(resolved);
        }

        [Test]
        public void GivenRegisterIfNotNull_WhenNotNullUnityObj_ThenRegistered()
        {
            // Arrange
            CreateContainerWith<ContainerRegisteringNotNullUnityObj>();

            // Act
            var resolved = Di.TryResolveGlobally<Collider>(out _);

            // Assert
            Assert.IsTrue(resolved);
        }

        [Test]
        public void GivenTryResolveGloballyAndRegister_WhenResolved_ThenRegistered()
        {
            // Arrange
            CreateContainerWith<ContainerRegisteringString>();
            CreateContainerWith<ContainerResolvingAndRegisteringString>();

            // Act
            var resolved = Di.TryResolveGlobally<string>(out _);

            // Assert
            Assert.IsTrue(resolved);
        }

        [Test]
        public void GivenTryResolveGloballyAndRegister_WhenNotResolved_ThenNotRegistered()
        {
            // Arrange
            CreateContainerWith<ContainerResolvingAndRegisteringString>();

            // Act
            var resolved = Di.TryResolveGlobally<string>(out _);

            // Assert
            Assert.IsFalse(resolved);
        }

        private class ContainerRegisteringNull : DependencyContainerBase
        {
            protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
            {
                builder.RegisterIfNotNull(null);
            }
        }

        private class ContainerRegisteringUnityNull : DependencyContainerBase
        {
            protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
            {
                var c = gameObject.AddComponent<BoxCollider>();
                DestroyImmediate(c); // using immediate so that object is null right away
                builder.RegisterIfNotNull(c);
            }
        }

        private class ContainerRegisteringNotNull : DependencyContainerBase
        {
            protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
            {
                builder.RegisterIfNotNull(new object());
            }
        }

        private class ContainerRegisteringNotNullUnityObj : DependencyContainerBase
        {
            protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
            {
                var c = gameObject.AddComponent<BoxCollider>();
                builder.RegisterIfNotNull(c);
            }
        }

        private class ContainerResolvingAndRegisteringString : DependencyContainerBase
        {
            protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
            {
                builder.TryResolveGloballyAndRegister<string>();
            }
        }

        private class ContainerRegisteringString : DependencyContainerBase
        {
            protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
            {
                builder.Register("Some string");
            }
        }
    }
}