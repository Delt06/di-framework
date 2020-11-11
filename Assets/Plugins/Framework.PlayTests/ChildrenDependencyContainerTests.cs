using Framework.Dependencies.Containers;
using NUnit.Framework;
using UnityEngine;

namespace Framework.PlayTests
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

			Assert.That(dependency.Rigidbody, Is.EqualTo(body));
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

			Assert.That(resolved, Is.False);
		}
	}
}