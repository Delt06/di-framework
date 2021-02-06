using DELTation.DIFramework.Containers;
using DELTation.DIFramework.Tests.Runtime.Components;
using NUnit.Framework;
using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime
{
	[TestFixture]
	public sealed class MultiplierRootContainersTests : TestFixtureBase
	{
		[Test]
		public void CreateTwoContainers_BothWithCorrectDependency_TheLatterIsTheActualSource()
		{
			CreateRigidbodyWithinContainer();
			var rigidbody2 = CreateRigidbodyWithinContainer();

			var component = CreateAndResolveRigidbodyComponent();

			Assert.That(component.Rigidbody != null);
			Assert.That(component.Rigidbody, Is.EqualTo(rigidbody2));
		}

		private Rigidbody CreateRigidbodyWithinContainer()
		{
			var container = CreateContainerWith<ListDependencyContainer>();
			var body = NewGameObject().AddComponent<Rigidbody>();
			container.Add(body);
			return body;
		}

		private RigidbodyComponent CreateAndResolveRigidbodyComponent()
		{
			var component = NewGameObject().AddComponent<RigidbodyComponent>();
			component.gameObject.AddComponent<Resolver>();
			return component;
		}

		[Test]
		public void CreateTwoContainers_OnlyFirstWithCorrectDependency_FirstIsTheSource()
		{
			var rigidbody = CreateRigidbodyWithinContainer();
			CreateContainerWith<ListDependencyContainer>();

			var component = CreateAndResolveRigidbodyComponent();

			Assert.That(component.Rigidbody != null);
			Assert.That(component.Rigidbody, Is.EqualTo(rigidbody));
		}

		[Test]
		public void CreateTwoContainers_OnlySecondWithCorrectDependency_SecondIsTheSource()
		{
			CreateContainerWith<ListDependencyContainer>();
			var rigidbody = CreateRigidbodyWithinContainer();

			var component = CreateAndResolveRigidbodyComponent();

			Assert.That(component.Rigidbody != null);
			Assert.That(component.Rigidbody, Is.EqualTo(rigidbody));
		}

		[Test]
		public void CreateManyContainers_OnlyOneWithCorrectDependency_ThatOneIsTheSource()
		{
			const int containerWithDependency = 2;
			Rigidbody rigidbody = null;
			for (var i = 0; i < 5; i++)
			{
				if (i == containerWithDependency)
					rigidbody = CreateRigidbodyWithinContainer();
				else
					CreateContainerWith<ListDependencyContainer>();
			}

			var component = CreateAndResolveRigidbodyComponent();

			Assert.That(component.Rigidbody != null);
			Assert.That(component.Rigidbody, Is.EqualTo(rigidbody));
		}
	}
}