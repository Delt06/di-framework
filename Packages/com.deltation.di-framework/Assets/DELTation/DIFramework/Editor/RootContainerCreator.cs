using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DELTation.DIFramework.Editor.MenuHelper;

namespace DELTation.DIFramework.Editor
{
    internal static class RootContainerCreator
    {
        [MenuItem(MenuName + "/Create Root Container", priority = 0)]
        public static void CreateRootContainer()
        {
            var go = new GameObject { name = "[Dependencies]" };
            go.AddComponent<RootDependencyContainer>();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}