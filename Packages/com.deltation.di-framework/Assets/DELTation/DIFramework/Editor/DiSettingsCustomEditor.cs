using System;
using System.Text;
using DELTation.DIFramework.Baking;
using DELTation.DIFramework.Editor.Baking;
using UnityEditor;
using UnityEngine;

namespace DELTation.DIFramework.Editor
{
    [CustomEditor(typeof(DiSettings))]
    public class DiSettingsCustomEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            _expandWidth = GUILayout.ExpandWidth(true);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();

            var boxStyle = GUI.skin.box;
            boxStyle.richText = true;

            _stringBuilder.Clear();

            if (!BakedInjection.DataExists)
            {
                _stringBuilder.Append("<color=yellow>Injection is not baked.</color>");
            }
            else
            {
                _stringBuilder.AppendLine("<color=green>Injection is baked.</color>");

                if (BakedInjection.BakedInjectionFunctionsCount == 0 &&
                    BakedInjection.BakedPocoInstantiationFunctionsCount == 0)
                    _stringBuilder.Append("<color=yellow>However, data is empty.</color>");
                else
                    _stringBuilder.Append("There is data for ")
                        .Append(BakedInjection.BakedInjectionFunctionsCount)
                        .Append(" component type(s) and ")
                        .Append(BakedInjection.BakedPocoInstantiationFunctionsCount)
                        .Append(" normal C# class(es).");
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Bake"))
            {
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
                InjectionBakingMenu.Bake();
            }

            if (GUILayout.Button("Clear Baked Data"))
            {
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
                InjectionBakingMenu.Clear();
            }

            GUILayout.EndHorizontal();

            GUILayout.Box(_stringBuilder.ToString(), boxStyle, _expandWidth);
        }

        private GUILayoutOption _expandWidth;
        private readonly StringBuilder _stringBuilder = new StringBuilder();
    }
}