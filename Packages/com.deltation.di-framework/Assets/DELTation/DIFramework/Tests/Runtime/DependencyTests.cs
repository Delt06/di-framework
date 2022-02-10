﻿using System.Linq;
using DELTation.DIFramework.Tests.Runtime.Containers;
using DELTation.DIFramework.Tests.Runtime.Pocos;
using NUnit.Framework;
using static DELTation.DIFramework.ContainerBuilder;

namespace DELTation.DIFramework.Tests.Runtime
{
    public class DependencyTests
    {
        [Test]
        public void GivenTypeAndObject_WhenHavingCtorWithObjectAsParameter_ThenDependsForward()
        {
            // Arrange
            var dependency1 = new Dependency(typeof(CtorInjectionContainer.StringDependent));
            var dependency2 = new Dependency("abc");

            // Act
            var dependsForward = Dependency.DependsOn(dependency1, dependency2);
            var dependsBack = Dependency.DependsOn(dependency2, dependency1);

            // Assert
            Assert.That(dependsForward);
            Assert.That(dependsBack, Is.False);
        }

        [Test]
        public void GivenTwoTypes_WhenHavingCtorWithOtherTypeAsParameter_ThenDependsForward()
        {
            // Arrange
            var dependency1 = new Dependency(typeof(PocoDep3));
            var dependency2 = new Dependency(typeof(PocoDep4));

            // Act
            var dependsForward = Dependency.DependsOn(dependency1, dependency2);
            var dependsBack = Dependency.DependsOn(dependency2, dependency1);

            // Assert
            Assert.That(dependsForward);
            Assert.That(dependsBack, Is.False);
        }

        [Test]
        public void GivenTypeAndObject_WhenHavingNoCtorParameters_ThenDoNotDepend()
        {
            // Arrange
            var dependency1 = new Dependency(typeof(object));
            var dependency2 = new Dependency("abc");

            // Act
            var dependsForward = Dependency.DependsOn(dependency1, dependency2);
            var dependsBack = Dependency.DependsOn(dependency2, dependency1);

            // Assert
            Assert.That(dependsForward, Is.False);
            Assert.That(dependsBack, Is.False);
        }

        [Test]
        public void GivenTypes_WhenHavingAllTypesAsParameters_ThenDependsForward()
        {
            // Arrange
            var dependency1 = new Dependency(typeof(Poco));
            var otherDependencies = new[]
            {
                typeof(PocoDep1),
                typeof(PocoDep2),
                typeof(PocoDep3),
            }.Select(t => new Dependency(t)).ToArray();

            // Act
            var dependForward =
                otherDependencies.Select(d => Dependency.DependsOn(dependency1, d));
            var dependsBack = otherDependencies.Select(d => Dependency.DependsOn(d, dependency1));

            // Assert
            Assert.That(dependForward, Is.All.True);
            Assert.That(dependsBack, Is.All.False);
        }

        [Test]
        public void GivenFactoryMethodAndObject_WhenHavingAllObjectTypeAsParameter_ThenDependsForward()
        {
            // Arrange
            var dependency1 =
                new Dependency(new FactoryMethod<CtorInjectionContainer.StringDependent, string>(str =>
                        new CtorInjectionContainer.StringDependent(str)
                    )
                );
            var dependency2 = new Dependency("abc");

            // Act
            var dependsForward = Dependency.DependsOn(dependency1, dependency2);
            var dependsBack = Dependency.DependsOn(dependency2, dependency1);

            // Assert
            Assert.That(dependsForward, Is.True);
            Assert.That(dependsBack, Is.False);
        }
    }
}