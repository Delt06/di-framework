using DELTation.DIFramework.Editor.Baking;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace DELTation.DIFramework.Editor
{
    public class BakeOnBuildPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!DiSettings.TryGetInstance(out var settings) || !settings.BakeOnBuild) return;

            Debug.Log("Baking DI injection...");
            InjectionBakingMenu.Bake();
            Debug.Log("Successfully baked DI injection.");
        }
    }
}