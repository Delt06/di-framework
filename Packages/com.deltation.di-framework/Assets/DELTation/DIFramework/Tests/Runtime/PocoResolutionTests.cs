using System;
using DELTation.DIFramework.Containers;
using DELTation.DIFramework.Tests.Runtime.Pocos;
using NUnit.Framework;

namespace DELTation.DIFramework.Tests.Runtime
{
    public class PocoResolutionTests
    {
        [Test]
        public void GivenDependencyContainerWithManyPocos_WhenGettingObject_ThenItIsCreatedAndInitialized()
        {
            // Arrange
            var container = new ConfigurableDependencyContainer(builder =>
                {
                    builder.Register<PocoDep4>();
                    builder.Register<Poco>();
                    builder.Register<PocoDep1>();
                    builder.Register<PocoDep2>();
                    builder.Register<PocoDep3>();
                }
            );

            // Act
            var resolved = container.TryResolve(typeof(Poco), out var pocoObject);
            var poco = (Poco) pocoObject;

            // Assert
            Assert.That(resolved);
            Assert.That(poco, Is.Not.Null);

            Assert.That(poco.Dep1, Is.Not.Null);
            Assert.That(poco.Dep2, Is.Not.Null);
            Assert.That(poco.Dep3, Is.Not.Null);

            Assert.That(poco.Dep1.Dep, Is.EqualTo(poco.Dep2));
            Assert.That(poco.Dep3.Dep, Is.Not.Null);
        }

        [Test]
        public void GivenDependencyContainerWithPoco_WhenGettingObject_ThenItIsCreatedAndInitialized()
        {
            // Arrange
            var container = new ConfigurableDependencyContainer(builder => { builder.Register<PocoDep4>(); }
            );

            // Act
            var resolved = container.TryResolve(typeof(PocoDep4), out var pocoObject);
            var poco = (PocoDep4) pocoObject;

            // Assert
            Assert.That(resolved);
            Assert.That(poco, Is.Not.Null);
        }

        [Test]
        public void GivenDependencyContainerWithSomePocos_WhenGettingObject_ThenItIsCreatedAndInitialized()
        {
            // Arrange
            var container = new ConfigurableDependencyContainer(builder =>
                {
                    builder.Register<PocoDep4>();
                    builder.Register<PocoDep3>();
                }
            );

            // Act
            var resolved = container.TryResolve(typeof(PocoDep3), out var pocoObject);
            var poco = (PocoDep3) pocoObject;

            // Assert
            Assert.That(resolved);
            Assert.That(poco, Is.Not.Null);

            Assert.That(poco.Dep, Is.Not.Null);
            container.TryResolve(typeof(PocoDep4), out var dep4);
            Assert.That(poco.Dep, Is.EqualTo(dep4));
        }

        [Test]
        public void
            GivenDependencyContainerWithSomePocosButDifferentOrder_WhenGettingObject_ThenItIsCreatedAndInitialized()
        {
            // Arrange
            var container = new ConfigurableDependencyContainer(builder =>
                {
                    builder.Register<PocoDep3>();
                    builder.Register<PocoDep4>();
                }
            );

            // Act
            var resolved = container.TryResolve(typeof(PocoDep3), out var pocoObject);
            var poco = (PocoDep3) pocoObject;

            // Assert
            Assert.That(resolved);
            Assert.That(pocoObject, Is.Not.Null);

            Assert.That(poco.Dep, Is.Not.Null);
            container.TryResolve(typeof(PocoDep4), out var dep4);
            Assert.That(poco.Dep, Is.EqualTo(dep4));
        }

        [Test]
        public void GivenDependencyContainerWithLoopedDependency_WhenGettingObject_ThenThrowsException()
        {
            // Arrange
            var container = new ConfigurableDependencyContainer(builder =>
                {
                    builder.Register<PocoLoop1>();
                    builder.Register<PocoLoop2>();
                }
            );

            // Act

            // Assert
            Assert.That(() => container.TryResolve(typeof(PocoLoop1), out _), Throws.Exception);
        }

        [Test]
        public void
            GivenDependencyContainerWithSomePocosWithInterface_WhenGettingObjectObject_ThenItIsCreatedAndInitialized()
        {
            // Arrange
            var container = new ConfigurableDependencyContainer(builder =>
                {
                    builder.Register<InterfaceDependant>();
                    builder.Register<InterfaceImpl>();
                }
            );

            // Act
            var resolved = container.TryResolve(typeof(InterfaceDependant), out var pocoObject);
            var dependant = (InterfaceDependant) pocoObject;

            // Assert
            Assert.That(resolved);
            Assert.That(dependant, Is.Not.Null);

            Assert.That(dependant.Interface, Is.Not.Null);
            container.TryResolve(typeof(InterfaceImpl), out var impl);
            Assert.That(dependant.Interface, Is.EqualTo(impl));
        }
    }
}