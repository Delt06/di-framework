﻿using Framework.Dependencies.Containers;
using Framework.PlayTests.Components;
using Framework.PlayTests.Containers;
using NUnit.Framework;

namespace Framework.PlayTests
{
	[TestFixture]
	public sealed class ResolverTests : TestFixtureBase
	{
		[Test]
		public void Resolver_WhenActivated_ConstructIsCalled()
		{
			var go = NewInactiveGameObject();
			var checker = go.AddComponent<ConstructChecker>();
			go.AddComponent<Resolver>();

			go.SetActive(true);

			Assert.That(checker.Constructed, Is.True);
		}

		[Test]
		public void AddResolver_WithOtherComponent_ResolverIsCalledFirst()
		{
			var go = NewInactiveGameObject();
			var componentBefore = go.AddComponent<DefaultExecutionOrderScript>();
			go.AddComponent<Resolver>();
			var componentAfter = go.AddComponent<DefaultExecutionOrderScript>();

			go.SetActive(true);

			Assert.That(componentBefore.AwakenWhenConstructed, Is.False);
			Assert.That(componentAfter.AwakenWhenConstructed, Is.False);
		}

		[Test]
		public void CreateObject_WithTwoResolvers_ConstructOnlyCalledOnce()
		{
			var root = NewInactiveGameObject();
			root.AddComponent<Resolver>();
			var child = NewGameObject();
			child.transform.parent = root.transform;
			child.AddComponent<Resolver>();
			var counter = child.AddComponent<ResolutionCounter>();

			root.SetActive(true);

			Assert.That(counter.Count, Is.EqualTo(1));
		}

		[Test]
		public void Resolver_ResolverByInterfaceInGameObject_Resolved()
		{
			var go = NewInactiveGameObject();
			var component = go.AddComponent<InterfaceDependencyComponent>();
			var implementation = go.AddComponent<InterfaceImplementation>();
			go.AddComponent<Resolver>();

			go.SetActive(true);

			Assert.That(component.Dependency, Is.EqualTo(implementation));
		}

		[Test]
		public void CreateObject_WithSeveralConstructors_ResolveAll()
		{
			var go = NewInactiveGameObject();
			var component = go.AddComponent<ComponentsWithSeveralConstructors>();
			go.AddComponent<Resolver>();
			CreateContainerWith<CustomContainer>();

			go.SetActive(true);

			Assert.That(component.FirstCalled);
			Assert.That(component.SecondCalled);
		}
	}
}