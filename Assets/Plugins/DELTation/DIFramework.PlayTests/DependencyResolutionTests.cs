using DELTation.DIFramework.PlayTests.Components;
using NUnit.Framework;
using UnityEngine;

namespace DELTation.DIFramework.PlayTests
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
	}
}