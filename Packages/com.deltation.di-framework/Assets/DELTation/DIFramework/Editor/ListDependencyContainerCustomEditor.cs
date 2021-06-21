using System.Linq;
using DELTation.DIFramework.Containers;
using UnityEditor;
using UnityEngine;
using static DELTation.DIFramework.ContainersExtensions;

namespace DELTation.DIFramework.Editor
{
    [CustomEditor(typeof(ListDependencyContainer))]
    internal class ListDependencyContainerCustomEditor : UnityEditor.Editor
    {
        private GUILayoutOption _miniButton;

        private void OnEnable()
        {
            _miniButton = GUILayout.Width(25);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var dependencies = serializedObject.FindProperty("_dependencies");
            DrawList(serializedObject, dependencies);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawList(SerializedObject container, SerializedProperty list)
        {
            for (var index = 0; index < list.arraySize; index++)
            {
                var currentValue = list.GetArrayElementAtIndex(index).objectReferenceValue;
                if (currentValue == null)
                    EditorGUILayout.HelpBox("Dependency cannot be null.", MessageType.Error, true);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(index), GUIContent.none);

                if (index == 0)
                    EditorGUI.BeginDisabledGroup(true);

                if (MiniButton("\u2191")) list.MoveArrayElement(index, index - 1);

                if (index == 0)
                    EditorGUI.EndDisabledGroup();

                if (index == list.arraySize - 1)
                    EditorGUI.BeginDisabledGroup(true);

                if (MiniButton("\u2193")) list.MoveArrayElement(index, index + 1);

                if (index == list.arraySize - 1)
                    EditorGUI.EndDisabledGroup();

                if (MiniButton("+")) list.InsertArrayElementAtIndex(index);

                if (MiniButton("-")) DeleteArrayElementAtIndex(list, index);

                if (currentValue is GameObject go)
                {
                    var size = GUILayout.Width(150);
                    var components = go.GetComponents<Component>()
                        .Where(c => c && !(c is Transform) && !ShouldBeIgnoredByContainer(c))
                        .ToArray();

                    var options = components.Select(c => c.GetType().Name).Prepend("<Select Component>").ToArray();
                    var selectedIndex = EditorGUILayout.Popup(0, options, size);
                    if (selectedIndex != 0)
                    {
                        var listDependencyContainer = (ListDependencyContainer) container.targetObject;
                        listDependencyContainer.Add(components[selectedIndex - 1]);
                        container.Update();
                        EditorUtility.SetDirty(listDependencyContainer);

                        list.DeleteArrayElementAtIndex(index);
                        list.DeleteArrayElementAtIndex(index);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            if (list.arraySize == 0 && GUILayout.Button("+"))
                list.arraySize++;
        }

        private bool MiniButton(string text) => GUILayout.Button(text, EditorStyles.miniButtonRight, _miniButton);

        private static void DeleteArrayElementAtIndex(SerializedProperty list, int index)
        {
            var oldSize = list.arraySize;
            list.DeleteArrayElementAtIndex(index);
            if (list.arraySize == oldSize)
                list.DeleteArrayElementAtIndex(index);
        }
    }
}