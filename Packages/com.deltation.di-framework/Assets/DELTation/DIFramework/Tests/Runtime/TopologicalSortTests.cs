using System;
using System.Collections.Generic;
using DELTation.DIFramework.Tests.Runtime.Containers;
using DELTation.DIFramework.Tests.Runtime.Pocos;
using NUnit.Framework;

namespace DELTation.DIFramework.Tests.Runtime
{
    public class TopologicalSortTests
    {
        private static bool MockResolutionFunction(Type type, out object obj)
        {
            obj = default;
            return false;
        }

        [Test]
        public void GivenTwoTypes_WhenOneDependsOnOther_ThenSortedAccordingly()
        {
            // Arrange
            var containerBuilder = new ContainerBuilder(MockResolutionFunction);
            containerBuilder.Register<Poco>();
            containerBuilder.Register<PocoDep2>();
            var result = new List<int>();

            // Act
            containerBuilder.SortTopologically(result, out var loop);

            // Assert
            Assert.That(loop, Is.False);
            Assert.That(result, Is.EquivalentTo(new[] { 0, 1 }));
        }

        [Test]
        public void GivenTypes_WhenThereIsTransitiveDependencyGraph_ThenSortedAccordingly()
        {
            // Arrange
            var containerBuilder = new ContainerBuilder(MockResolutionFunction);
            containerBuilder.Register<Poco>();
            containerBuilder.Register<PocoDep4>();
            containerBuilder.Register<PocoDep1>();
            containerBuilder.Register<PocoDep2>();
            containerBuilder.Register<PocoDep3>();
            var result = new List<int>();

            // Act
            containerBuilder.SortTopologically(result, out var loop);

            // Assert
            Assert.That(loop, Is.False);
            Assert.That(result, Is.EquivalentTo(new[] { 0, 2, 3, 4, 1 }));
        }

        [Test]
        public void GivenTypeAndObject_WhenTypeDependsOnObject_ThenSortedAccordingly()
        {
            // Arrange
            var containerBuilder = new ContainerBuilder(MockResolutionFunction);
            containerBuilder.Register("abc");
            containerBuilder.Register<CtorInjectionContainer.StringDependent>();
            var result = new List<int>();

            // Act
            containerBuilder.SortTopologically(result, out var loop);

            // Assert
            Assert.That(loop, Is.False);
            Assert.That(result, Is.EquivalentTo(new[] { 1, 0 }));
        }

        [Test]
        public void GivenTypesAndFactorMethod_WhenTransitiveDependencyGraph_ThenSortedAccordingly()
        {
            // Arrange
            var containerBuilder = new ContainerBuilder(MockResolutionFunction);
            containerBuilder.Register<PocoDep1>();
            containerBuilder.Register<PocoDep2>();
            containerBuilder.Register<PocoDep3>();
            containerBuilder.Register<PocoDep4>();
            containerBuilder.RegisterFromMethod((PocoDep1 dep1, PocoDep2 dep2, PocoDep3 dep3) =>
                new Poco(dep1, dep2, dep3)
            );
            var result = new List<int>();

            // Act
            containerBuilder.SortTopologically(result, out var loop);

            // Assert
            Assert.That(loop, Is.False);
            Assert.That(result, Is.EquivalentTo(new[] { 4, 0, 1, 2, 3 }));
        }
    }
}