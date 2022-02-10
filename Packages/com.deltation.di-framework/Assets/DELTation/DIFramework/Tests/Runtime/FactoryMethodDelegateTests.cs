using System;
using DELTation.DIFramework.Exceptions;
using DELTation.DIFramework.Tests.Runtime.Containers;
using NUnit.Framework;

namespace DELTation.DIFramework.Tests.Runtime
{
    public class FactoryMethodDelegateTests
    {
        private static bool MockResolutionFunction(Type type, out object obj)
        {
            obj = default;
            return false;
        }

        [Test]
        public void GivenFactoryMethodDelegate_WhenHavingNoDependencies_ThenCanCreate()
        {
            // Arrange
            const string createdObject = "abc";
            var factoryMethodDelegate = new FactoryMethodDelegate(new FactoryMethod<string>(() => createdObject));

            // Act
            var obj = factoryMethodDelegate.Instantiate(MockResolutionFunction);

            // Assert
            Assert.That(obj, Is.EqualTo(createdObject));
        }

        [Test]
        public void GivenFactoryMethodDelegate_WhenHavingOneDependency_ThenCanCreate()
        {
            // Arrange
            const string dependency = "abc";
            var factoryMethodDelegate = new FactoryMethodDelegate(
                new FactoryMethod<CtorInjectionContainer.StringDependent, string>(str =>
                    new CtorInjectionContainer.StringDependent(str)
                )
            );

            // Act
            static bool StringResolutionFunction(Type type, out object obj)
            {
                if (type == typeof(string))
                {
                    obj = dependency;
                    return true;
                }

                obj = default;
                return false;
            }

            var obj = factoryMethodDelegate.Instantiate(StringResolutionFunction);

            // Assert
            Assert.That(obj, Is.TypeOf<CtorInjectionContainer.StringDependent>());
            Assert.That(((CtorInjectionContainer.StringDependent) obj).S, Is.EqualTo(dependency));
        }

        [Test]
        public void GivenFactoryMethodDelegate_WhenHavingOneUnmetDependency_ThenThrowsDependencyNotRegisteredException()
        {
            // Arrange
            var factoryMethodDelegate = new FactoryMethodDelegate(
                new FactoryMethod<CtorInjectionContainer.StringDependent, string>(str =>
                    new CtorInjectionContainer.StringDependent(str)
                )
            );

            // Act
            void TestedCode()
            {
                factoryMethodDelegate.Instantiate(MockResolutionFunction);
            }

            // Assert
            Assert.Throws<DependencyNotRegisteredException>(TestedCode);
        }
    }
}