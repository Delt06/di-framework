using System;
using DELTation.DIFramework.Resolution;
using NUnit.Framework;
using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime
{
    public class PocoInjectionTests
    {
        [Test, TestCase(typeof(string), true), TestCase(typeof(MonoBehaviour), false)]
        public void GivenIsPoco_WhenDerivesFromUnityObject_ThenIsPocoIsTrue(Type type, bool expectIsPoco)
        {
            // Arrange

            // Act
            var isPoco = PocoInjection.IsPoco(type);

            // Assert
            Assert.That(isPoco, Is.EqualTo(expectIsPoco));
        }

        [Test, TestCase(typeof(InjectablePoco), true), TestCase(typeof(NotInjectablePoco), false)]
        public void GivenIsInjectable_WhenHasInjectableConstructorParameters_ThenIsInjectableIsTrue(Type type,
            bool expectIsInjectable)
        {
            // Arrange

            // Act
            var isInjectable = PocoInjection.IsInjectable(type);

            // Assert
            Assert.That(isInjectable, Is.EqualTo(expectIsInjectable));
        }

        private class InjectablePoco
        {
            public InjectablePoco(string str) { }
        }

        private class NotInjectablePoco
        {
            public NotInjectablePoco(int i) { }
        }
    }
}