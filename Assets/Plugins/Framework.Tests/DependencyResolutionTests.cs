using System.Collections.Generic;
using Framework.Core;
using Framework.Dependencies;
using Framework.Dependencies.Containers;
using NUnit.Framework;
using UnityEngine;

namespace Framework.Tests
{
	[TestFixture]
	public class LocalDependencyResolutionTests
	{
		private readonly List<GameObject> _gameObjects = new List<GameObject>();

		[TearDown]
		public void TearDown()
		{
			foreach (var gameObject in _gameObjects)
			{
				Object.DestroyImmediate(gameObject);
			}

			_gameObjects.Clear();
		}

		private GameObject NewGameObject()
		{
			var go = new GameObject();
			_gameObjects.Add(go);
			return go;
		}

		[Test]
		public void CreateObject_WithLocalDependency_Resolved()
		{
			var go = NewGameObject();
			var body = go.AddComponent<Rigidbody>();

			var dependency = go.AddComponent<LocalDependencyComponent>();

			Assert.That(dependency.Rigidbody, Is.EqualTo(body));
		}

		[Test]
		public void CreateObject_WithParentDependency_Resolved()
		{
			var go = NewGameObject();
			var child = NewGameObject();
			child.transform.parent = go.transform;
			var body = go.AddComponent<Rigidbody>();

			var dependency = child.AddComponent<ParentDependencyComponent>();

			Assert.That(dependency.Rigidbody, Is.EqualTo(body));
		}

		[Test]
		public void CreateObject_WithChildDependency_Resolved()
		{
			var go = NewGameObject();
			var child = NewGameObject();
			child.transform.parent = go.transform;
			var body = child.AddComponent<Rigidbody>();

			var dependency = go.AddComponent<ChildrenDependencyComponent>();

			Assert.That(dependency.Rigidbody, Is.EqualTo(body));
		}

		[Test]
		public void CreateObject_WithEntityDependency_Resolved()
		{
			var go = NewGameObject();
			go.AddComponent<Entity>();
			var child = NewGameObject();
			child.transform.parent = go.transform;
			var body = go.AddComponent<Rigidbody>();

			var dependency = child.AddComponent<EntityDependencyComponent>();

			Assert.That(dependency.Rigidbody, Is.EqualTo(body));
		}

		[Test]
		public void CreateObject_WithGlobalDependency_ResolvedByList()
		{
			var root = NewGameObject().AddComponent<RootDependencyContainer>();
			var body = NewGameObject().AddComponent<Rigidbody>();
			var container = NewGameObject().AddComponent<ListDependencyContainer>();
			container.Dependencies.Add(body);
			container.transform.parent = root.transform;

			var go = NewGameObject();
			go.AddComponent<Entity>();
			var dependency = go.AddComponent<GlobalDependencyComponent>();

			Assert.That(dependency.Rigidbody, Is.EqualTo(body));
		}

		[Test]
		public void CreateObject_WithGlobalDependency_ResolvedByFallback()
		{
			var root = NewGameObject().AddComponent<RootDependencyContainer>();
			var body = NewGameObject().AddComponent<Rigidbody>();
			body.name = "Rigidbody";
			var container = NewGameObject().AddComponent<FallbackDependencyContainer>();
			container.transform.parent = root.transform;

			var go = NewGameObject();
			go.AddComponent<Entity>();
			var dependency = go.AddComponent<GlobalDependencyComponent>();

			Assert.That(dependency.Rigidbody, Is.EqualTo(body));
		}

		[Test]
		public void CreateObject_WithGlobalDependency_ResolvedByChildren()
		{
			var root = NewGameObject().AddComponent<RootDependencyContainer>();
			var container = NewGameObject().AddComponent<ChildrenDependencyContainer>();
			var body = container.gameObject.AddComponent<Rigidbody>();
			container.transform.parent = root.transform;

			var go = NewGameObject();
			go.AddComponent<Entity>();
			var dependency = go.AddComponent<GlobalDependencyComponent>();

			Assert.That(dependency.Rigidbody, Is.EqualTo(body));
		}
	}
}