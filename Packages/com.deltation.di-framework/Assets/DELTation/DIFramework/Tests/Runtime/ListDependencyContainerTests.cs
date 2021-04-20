using DELTation.DIFramework.Containers;
using DELTation.DIFramework.Tests.Runtime.Components;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime
{
    [TestFixture]
    public class ListDependencyContainerTests : TestFixtureBase
    {
        [Test]
        public void CreateObject_WithDependency_Resolved()
        {
            var body = NewGameObject().AddComponent<Rigidbody>();
            var container = CreateContainerWith<ListDependencyContainer>();
            container.Add(body);

            var go = NewGameObject();
            var dependency = go.AddComponent<RigidbodyComponent>();
            go.AddComponent<Resolver>();

            dependency.Rigidbody.Should().Be(body);
        }

        [Test]
        public void CreateObject_WithIgnoredDependency_NotResolved()
        {
            var container = CreateContainerWith<ListDependencyContainer>();
            var ignoredBody = NewGameObject()
                .AddComponent<Rigidbody>()
                .gameObject.AddComponent<IgnoreByContainer>();
            container.Add(ignoredBody);

            var resolved = container.TryResolve(out Rigidbody _);

            resolved.Should().BeFalse();
        }

        [Test]
        public void CreateObjects_WithCyclicDependency_BothResolved()
        {
            var container = CreateContainerWith<ListDependencyContainer>();
            var go1 = NewGameObject();
            go1.SetActive(false);
            var component1 = go1.AddComponent<LoopComponent1>();
            go1.AddComponent<Resolver>();
            var go2 = new GameObject();
            go2.SetActive(false);
            var component2 = go2.AddComponent<LoopComponent2>();
            go2.AddComponent<Resolver>();
            container.Add(component1);
            container.Add(component2);

            go1.SetActive(true);
            go2.SetActive(true);

            component1.Component.Should().Be(component2);
            component2.Component.Should().Be(component1);
        }
    }
}