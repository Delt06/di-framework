using System;
using System.Collections.Generic;
using DELTation.DIFramework.Dependencies;
using DELTation.DIFramework.Tests.Runtime.Containers;
using NUnit.Framework;

namespace DELTation.DIFramework.Tests.Runtime
{
    public class DependencyUtilsTests
    {
        [Test]
        public void GivenDependenciesCanBeResolved_WhenNoDependencies_ThenReturnsTrue()
        {
            // Arrange
            var dependency = new ObjectDependency(new object());

            // Act
            var dependenciesCanBeResolved =
                DependencyUtils.DependenciesCanBeResolved(dependency, new List<Type>(), new List<Type>());

            // Assert
            Assert.That(dependenciesCanBeResolved);
        }

        [Test]
        public void GivenDependenciesCanBeResolved_WhenDependencyIsAmongResolvers_ThenReturnsTrue()
        {
            // Arrange
            var dependency = new TypeDependency(typeof(CtorInjectionContainer.StringDependent));

            // Act
            var dependenciesCanBeResolved =
                DependencyUtils.DependenciesCanBeResolved(dependency, new List<Type>
                    {
                        typeof(string),
                    }, new List<Type>()
                );

            // Assert
            Assert.That(dependenciesCanBeResolved);
        }

        [Test]
        public void GivenDependenciesCanBeResolved_WhenDependencyIsNotAmongResolvers_ThenReturnsFalse()
        {
            // Arrange
            var dependency = new TypeDependency(typeof(CtorInjectionContainer.StringDependent));
            var unresolvedDependencies = new List<Type>();

            // Act
            var dependenciesCanBeResolved =
                DependencyUtils.DependenciesCanBeResolved(dependency, new List<Type>(), unresolvedDependencies);

            // Assert
            Assert.That(dependenciesCanBeResolved, Is.False);
            Assert.That(unresolvedDependencies, Is.EquivalentTo(new[] { typeof(string) }));
        }
    }
}