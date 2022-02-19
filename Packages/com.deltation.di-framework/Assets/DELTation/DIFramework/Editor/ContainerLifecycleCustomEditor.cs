#if ODIN_INSPECTOR
using EditorBase = Sirenix.OdinInspector.Editor.OdinEditor;
#else
using EditorBase = UnityEditor.Editor;
#endif
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DELTation.DIFramework.Editor
{
    [CustomEditor(typeof(ContainerLifecycle), true)]
    public class ContainerLifecycleCustomEditor : EditorBase
    {
        private bool _foldout;
        private GUIStyle _labelStyle;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying) return;

            _foldout = EditorGUILayout.Foldout(_foldout, "Affected Objects");
            if (!_foldout) return;

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                richText = true,
            };

            var containerLifecycle = (ContainerLifecycle) target;
            EditorGUI.indentLevel++;
            DrawObjectsList("<b>IStartable</b>", containerLifecycle.Startables);
            DrawObjectsList("<b>IUpdatable</b>", containerLifecycle.Updatables);
            DrawObjectsList("<b>IFixedUpdatable</b>", containerLifecycle.FixedUpdatables);
            DrawObjectsList("<b>ILateUpdatable</b>", containerLifecycle.LateUpdatables);
            DrawObjectsList("<b>IDestroyable</b>", containerLifecycle.Destroyables);
            EditorGUI.indentLevel--;
        }

        private void DrawObjectsList(string headerLabel, IReadOnlyList<object> objects)
        {
            if (objects.Count == 0) return;
            Label(headerLabel);
            EditorGUI.indentLevel++;

            for (var index = 0; index < objects.Count; index++)
            {
                var startable = objects[index];
                Label(startable.GetType().GetFriendlyName());
            }

            EditorGUI.indentLevel--;
        }

        private void Label(string label)
        {
            EditorGUILayout.LabelField(label, _labelStyle);
        }
    }
}