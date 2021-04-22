using System;
using DELTation.DIFramework.Containers;
using DELTation.DIFramework.Tests.Runtime.Pocos;
using FluentAssertions;
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
            resolved.Should().BeTrue();
            poco.Should().NotBeNull();

            poco.Dep1.Should().NotBeNull();
            poco.Dep2.Should().NotBeNull();
            poco.Dep3.Should().NotBeNull();

            poco.Dep1.Dep.Should().Be(poco.Dep2);
            poco.Dep3.Dep.Should().NotBeNull();
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
            resolved.Should().BeTrue();
            poco.Should().NotBeNull();
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
            resolved.Should().BeTrue();
            poco.Should().NotBeNull();

            poco.Dep.Should().NotBeNull();
            container.TryResolve(typeof(PocoDep4), out var dep4);
            poco.Dep.Should().Be(dep4);
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
            resolved.Should().BeTrue();
            poco.Should().NotBeNull();

            poco.Dep.Should().NotBeNull();
            container.TryResolve(typeof(PocoDep4), out var dep4);
            poco.Dep.Should().Be(dep4);
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
            Action action = () => container.TryResolve(typeof(PocoLoop1), out _);
            action.Should().Throw<Exception>();
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
            resolved.Should().BeTrue();
            dependant.Should().NotBeNull();

            dependant.Interface.Should().NotBeNull();
            container.TryResolve(typeof(InterfaceImpl), out var impl);
            dependant.Interface.Should().Be(impl);
        }
    }
}