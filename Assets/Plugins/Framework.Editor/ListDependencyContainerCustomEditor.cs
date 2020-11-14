using Framework.Dependencies.Containers;
using UnityEditor;
using UnityEngine;

namespace Plugins.Framework.Editor
{
	[CustomEditor(typeof(ListDependencyContainer))]
	public class ListDependencyContainerCustomEditor : UnityEditor.Editor
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
			DrawList(dependencies);

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawList(SerializedProperty list)
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