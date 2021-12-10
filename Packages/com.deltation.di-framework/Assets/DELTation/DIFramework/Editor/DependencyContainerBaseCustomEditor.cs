using DELTation.DIFramework.Containers;
using UnityEditor;

#if ODIN_INSPECTOR
using EditorBase = Sirenix.OdinInspector.Editor.OdinEditor;
#else
using EditorBase = UnityEditor.Editor;
#endif

namespace DELTation.DIFramework.Editor
{
    [CustomEditor(typeof(DependencyContainerBase), true, isFallback = true)]
    public class DependencyContainerBaseCustomEditor : EditorBase
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!(target is DependencyContainerBase container)) return;

            const bool wide = true;
            var hasLoops = container.HasLoops();
            if (hasLoops)
                EditorGUILayout.HelpBox("Dependency graph contains a loop.", MessageType.Error, wide);
            else
                EditorGUILayout.HelpBox("Dependency graph is valid.", MessageType.Info, wide);
        }
    }
}