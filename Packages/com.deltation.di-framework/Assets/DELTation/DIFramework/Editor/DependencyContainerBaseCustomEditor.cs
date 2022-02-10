#if ODIN_INSPECTOR
using EditorBase = Sirenix.OdinInspector.Editor.OdinEditor;
#else
using EditorBase = UnityEditor.Editor;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using DELTation.DIFramework.Containers;
using UnityEditor;

namespace DELTation.DIFramework.Editor
{
    [CustomEditor(typeof(DependencyContainerBase), true, isFallback = true)]
    public class DependencyContainerBaseCustomEditor : EditorBase
    {
        private readonly List<(Type dependent, Type unresolvedDependency)> _unresolvedDependencies =
            new List<(Type dependent, Type unresolvedDependency)>();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!(target is DependencyContainerBase container)) return;

            const bool wide = true;
            var hasLoops = container.HasLoops();
            if (hasLoops)
            {
                EditorGUILayout.HelpBox("Dependency graph contains a loop.", MessageType.Error, wide);
            }
            else
            {
                _unresolvedDependencies.Clear();
                if (container.DependenciesCanBeResolved(_unresolvedDependencies))
                {
                    EditorGUILayout.HelpBox("Dependency graph is valid.", MessageType.Info, wide);
                }
                else
                {
                    var message = ComposeUnresolvedDependenciesMessage();
                    EditorGUILayout.HelpBox(message, MessageType.Error, wide);
                }
            }
        }

        private string ComposeUnresolvedDependenciesMessage()
        {
            return string.Join("\n",
                _unresolvedDependencies.Select(t =>
                    $"{t.dependent.GetFriendlyName()} could not resolve a dependency of type {t.unresolvedDependency.GetFriendlyName()}."
                )
            );
        }
    }
}