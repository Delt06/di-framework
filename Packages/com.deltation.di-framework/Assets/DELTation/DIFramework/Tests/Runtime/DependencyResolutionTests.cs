using DELTation.DIFramework.Tests.Runtime.Components;
using NUnit.Framework;
using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime
{
    [TestFixture]
    public class LocalDependencyResolutionTests : TestFixtureBase
    {
        [Test]
        public void CreateObject_WithDependency_ResolvedLocally()
        {
            var go = NewGameObject();
            var body = go.AddComponent<Rigidbody>();

            var dependency = go.AddComponent<RigidbodyComponent>();
            go.AddComponent<Resolver>();

            Assert.That(dependency.Rigidbody, Is.EqualTo(body));
        }

        [Test]
        public void CreateObject_WithDependency_ResolvedInParent()
        {
            var go = NewGameObject();
            var child = NewGameObject();
            child.transform.parent = go.transform;
            var body = go.AddComponent<Rigidbody>();

            var dependency = child.AddComponent<RigidbodyComponent>();
            go.AddComponent<Resolver>();

            Assert.That(dependency.Rigidbody, Is.EqualTo(body));
        }

        [Test]
        public void CreateObject_WithDependency_ResolvedInChildren()
        {
            var go = NewGameObject();
            var child = NewGameObject();
            child.transform.parent = go.transform;
            var body = child.AddComponent<Rigidbody>();

            var dependency = go.AddComponent<RigidbodyComponent>();
            go.AddComponent<Resolver>();

            Assert.That(dependency.Rigidbody, Is.EqualTo(body));
        }

        [Test]
        public void CreateObjectWithTwoChildren_WithLocalDependencies_BothResolvedLocally()
        {
            var parent = NewGameObject();

            var child1 = NewGameObject();
            child1.transform.parent = parent.transform;
            var rigidbody1 = child1.AddComponent<Rigidbody>();
            var rigidbodyComponent1 = child1.AddComponent<RigidbodyComponent>();

            var child2 = NewGameObject();
            child2.transform.parent = parent.transform;
            var rigidbody2 = child2.AddComponent<Rigidbody>();
            var rigidbodyComponent2 = child2.AddComponent<RigidbodyComponent>();

            parent.AddComponent<Resolver>();

            Assert.That(rigidbodyComponent1.Rigidbody, Is.EqualTo(rigidbody1));
            Assert.That(rigidbodyComponent2.Rigidbody, Is.EqualTo(rigidbody2));
        }
    }
}