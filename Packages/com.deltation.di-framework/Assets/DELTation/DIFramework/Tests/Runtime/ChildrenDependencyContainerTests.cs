using DELTation.DIFramework.Containers;
using DELTation.DIFramework.Tests.Runtime.Components;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime
{
    [TestFixture]
    public class ChildrenDependencyContainerTests : TestFixtureBase
    {
        [Test]
        public void CreateObject_WithDependency_Resolved()
        {
            var container = CreateContainerWith<ChildrenDependencyContainer>();
            var body = container.gameObject.AddComponent<Rigidbody>();

            var dependency = CreateRigidbodyComponent();

            dependency.Rigidbody.Should().Be(body);
        }

        private RigidbodyComponent CreateRigidbodyComponent()
        {
            var go = NewGameObject();
            var dependency = go.AddComponent<RigidbodyComponent>();
            go.AddComponent<Resolver>();
            return dependency;
        }

        [Test]
        public void CreateObject_WithIgnoredDependency_NotResolved()
        {
            var ignoredBody = NewGameObject()
                .gameObject.AddComponent<IgnoreByContainer>()
                .gameObject.AddComponent<Rigidbody>();
            var container = CreateContainerWith<ChildrenDependencyContainer>();
            ignoredBody.transform.parent = container.transform;

            var resolved = container.TryResolve(out Rigidbody _);

            resolved.Should().BeFalse();
        }
    }
}