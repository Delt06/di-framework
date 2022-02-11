using System.Linq;
using DELTation.DIFramework.Containers;
using DELTation.DIFramework.Tests.Runtime.Containers;
using NUnit.Framework;

namespace DELTation.DIFramework.Tests.Runtime
{
    [TestFixture]
    public class InternalDependencyTests : TestFixtureBase
    {
        private const string String = "abc";

        public class InternalDependencyContainer : DependencyContainerBase
        {
            protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
            {
                builder
                    .Register(String).AsInternal()
                    .Register<CtorInjectionContainer.StringDependent>()
                    .RegisterIfNotNull(null).AsInternal()
                    ;
            }
        }

        public class InternalDependencyContainerInReverseOrder : DependencyContainerBase
        {
            protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
            {
                builder
                    .Register<CtorInjectionContainer.StringDependent>()
                    .Register(String).AsInternal()
                    ;
            }
        }

        [Test]
        public void GivenContainer_WhenRegistering_ThenHasOneExternalDependency()
        {
            // Arrange
            var container = CreateContainerWith<InternalDependencyContainer>();

            // Act
            var allRegisteredObjects = container.GetAllRegisteredObjects().ToArray();

            // Assert
            Assert.That(allRegisteredObjects.Length, Is.EqualTo(1));
            Assert.That(allRegisteredObjects[0], Is.TypeOf<CtorInjectionContainer.StringDependent>());
        }

        [Test]
        public void GivenContainer_WhenRegisteringAsInternal_ThenCanResolveInternally()
        {
            // Arrange
            var container = CreateContainerWith<InternalDependencyContainer>();

            // Act
            var canResolve = container.TryResolve(out CtorInjectionContainer.StringDependent stringDependent);

            // Assert
            Assert.That(canResolve);
            Assert.That(stringDependent.S, Is.EqualTo(String));
        }

        [Test]
        public void GivenContainer_WhenRegisteringAsInternal_ThenCannotResolveFromOutside()
        {
            // Arrange
            var container = CreateContainerWith<InternalDependencyContainer>();

            // Act
            var canResolve = container.TryResolve<string>(out _);

            // Assert
            Assert.That(canResolve, Is.False);
        }

        [Test]
        public void GivenContainerInReverseOrder_WhenRegisteringAsInternal_ThenCanResolveInternally()
        {
            // Arrange
            var container = CreateContainerWith<InternalDependencyContainerInReverseOrder>();

            // Act
            var canResolve = container.TryResolve(out CtorInjectionContainer.StringDependent stringDependent);

            // Assert
            Assert.That(canResolve);
            Assert.That(stringDependent.S, Is.EqualTo(String));
        }

        [Test]
        public void GivenContainerInReverseOrder_WhenRegisteringAsInternal_ThenCannotResolveFromOutside()
        {
            // Arrange
            var container = CreateContainerWith<InternalDependencyContainerInReverseOrder>();

            // Act
            var canResolve = container.TryResolve<string>(out _);

            // Assert
            Assert.That(canResolve, Is.False);
        }
    }
}