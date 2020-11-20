using Framework.Dependencies;
using Framework.Dependencies.Containers;
using Framework.PlayTests.Components;
using NUnit.Framework;
using UnityEngine;

namespace Framework.PlayTests
{
	[TestFixture]
	public class FallbackDependencyContainerTests : TestFixtureBase
	{
		[Test]
		public void CreateObject_WithDependency_Resolved()
		{
			var body = NewGameObject("Rigidbody").AddComponent<Rigidbody>();
			CreateContainerWith<FallbackDependencyContainer>();

			var go = NewGameObject();
			var dependency = go.AddComponent<RigidbodyComponent>();
			go.AddComponent<Resolver>();

			Assert.That(dependency.Rigidbody, Is.EqualTo(body));
		}

		[Test]
		public void CreateObject_WithIgnoredDependency_NotResolved()
		{
			NewGameObject()
				.gameObject.AddComponent<IgnoreByContainer>()
				.gameObject.AddComponent<Rigidbody>();
			var container = CreateContainerWith<FallbackDependencyContainer>();

			var resolved = container.TryResolve(out Rigidbody _);

			Assert.That(resolved, Is.False);
		}
	}
}