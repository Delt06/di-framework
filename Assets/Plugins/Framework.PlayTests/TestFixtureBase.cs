using System.Collections.Generic;
using Framework.Dependencies;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;

namespace Framework.PlayTests
{
	public class TestFixtureBase
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

		protected GameObject NewGameObject([CanBeNull] string name = null)
		{
			var go = new GameObject();
			
			if (name != null)
				go.name = name;

			_gameObjects.Add(go);
			return go;
		}

		protected GameObject NewInactiveGameObject()
		{
			var go = NewGameObject();
			go.SetActive(false);
			return go;
		}

		protected T CreateContainerWith<T>() where T : Component
		{
			var root = NewGameObject().AddComponent<RootDependencyContainer>();
			var component = NewGameObject().AddComponent<T>();
			component.transform.parent = root.transform;
			return component;
		}
	}
}