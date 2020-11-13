using Framework.Dependencies.Containers;
using Framework.PlayTests.Components;
using NUnit.Framework;
using UnityEngine;

namespace Framework.PlayTests
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

			Assert.That(dependency.Rigidbody, Is.EqualTo(body));
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

			Assert.That(resolved, Is.False);
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

			Assert.That(component1.Component, Is.EqualTo(component2));
			Assert.That(component2.Component, Is.EqualTo(component1));
		}
	}
}