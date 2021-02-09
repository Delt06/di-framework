using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DELTation.DIFramework.Editor
{
	public static class RootContainerCreator
	{
		[MenuItem("DI/Create Root Container", priority = 0)]
		public static void CreateRootContainer()
		{
			var go = new GameObject {name = "[Dependencies]"};
			go.AddComponent<RootDependencyContainer>();
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}

		[MenuItem("DI/Create Root Container", true)]
		public static bool ThereIsNotRootContainer()
		{
			return Object.FindObjectOfType<RootDependencyContainer>() == null;
		}
	}
}