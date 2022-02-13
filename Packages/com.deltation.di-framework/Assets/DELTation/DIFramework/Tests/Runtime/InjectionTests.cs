using System;
using DELTation.DIFramework.Resolution;
using DELTation.DIFramework.Tests.Runtime.Components;
using NUnit.Framework;
using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime
{
    public class InjectionTests
    {
        [Test]
        public void GivenInvalidatedInjection_WhenCheckingCache_ThenItIsEmpty()
        {
            // Arrange
            Injection.InvalidateCache();

            // Act

            // Assert
            Assert.That(Injection.CacheIsEmpty());
        }

        [Test]
        public void GivenInvalidatedInjection_WhenWarmingUpForGameObject_ThenItIsNotEmpty()
        {
            // Arrange
            Injection.InvalidateCache();
            var gameObject = new GameObject();
            gameObject.AddComponent<RigidbodyComponent>();

            // Act
            Injection.WarmUp(gameObject);

            // Assert
            Assert.That(Injection.CacheIsEmpty(), Is.False);
        }

        [Test]
        public void GivenInvalidatedInjection_WhenWarmingUpSeveralTypes_ThenItIsNotEmpty()
        {
            // Arrange
            Injection.InvalidateCache();

            // Act
            Injection.WarmUp(typeof(RigidbodyComponent), typeof(ConstructChecker));

            // Assert
            Assert.That(Injection.CacheIsEmpty(), Is.False);
        }

        [Test, TestCase(typeof(RigidbodyComponent), new[] { typeof(Rigidbody) }),
         TestCase(typeof(ResolutionCounter), new Type[] { }),
         TestCase(typeof(ComponentsWithSeveralConstructors), new[] { typeof(Parent), typeof(Child) })]
        public void GivenInjection_WhenGettingAllDependencies_ThenAllConstructParametersArePresent(Type componentType,
            Type[] expectedDependencies)
        {
            // Arrange

            // Act
            var dependencies = Injection.GetAllDependenciesOf(componentType);

            // Assert
            Assert.That(dependencies, Is.EquivalentTo(expectedDependencies));
        }

        [Test]
        public void GivenInjectableComponent_WhenCallingIsInjectable_ThenReturnsTrue()
        {
            // Arrange

            // Act
            var isInjectable = Injection.IsInjectable(typeof(RigidbodyComponent));

            // Assert
            Assert.That(isInjectable);
        }

        [Test, TestCase(typeof(ValueTypeDependencyComponent)), TestCase(typeof(RefParameterDependencyComponent)),
         TestCase(typeof(InParameterDependencyComponent)), TestCase(typeof(OutParameterDependencyComponent))]
        public void GivenNonInjectableComponent_WhenCallingIsInjectable_ThenReturnsFalse(Type type)
        {
            // Arrange

            // Act
            var isInjectable = Injection.IsInjectable(type);

            // Assert
            Assert.That(isInjectable, Is.False);
        }

        [Test]
        public void GivenObjectWithDeepHierarchy_WhenGettingAffectedComponents_ThenAllAreReturnedWithCorrectDepths()
        {
            // Arrange
            var root = new GameObject();
            var rigidbodyComponent = root.AddComponent<RigidbodyComponent>();
            var child = new GameObject
            {
                transform = { parent = root.transform },
            };
            var constructChecker = child.AddComponent<ConstructChecker>();

            // Act
            var affectedComponents = Injection.GetAffectedComponents(root.transform);

            // Assert
            Assert.That(affectedComponents,
                Is.EqualTo(new (MonoBehaviour component, int depth)[] { (rigidbodyComponent, 0), (constructChecker, 1) }
                )
            );
        }

        private class ValueTypeDependencyComponent : MonoBehaviour
        {
            // ReSharper disable once UnusedMember.Local
            // ReSharper disable once UnusedParameter.Local
            public void Construct(int i) { }
        }

        private class RefParameterDependencyComponent : MonoBehaviour
        {
            // ReSharper disable once UnusedMember.Local
            // ReSharper disable once UnusedParameter.Local
            public void Construct(ref string s) { }
        }

        private class InParameterDependencyComponent : MonoBehaviour
        {
            // ReSharper disable once UnusedMember.Local
            // ReSharper disable once UnusedParameter.Local
            public void Construct(in string s) { }
        }

        private class OutParameterDependencyComponent : MonoBehaviour
        {
            // ReSharper disable once UnusedMember.Local
            public void Construct(out string s)
            {
                s = default;
            }
        }
    }
}