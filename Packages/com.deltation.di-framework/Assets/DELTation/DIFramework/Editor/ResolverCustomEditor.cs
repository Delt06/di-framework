using System;
using System.Text;
using DELTation.DIFramework.Reporting;
using UnityEditor;
using UnityEngine;

namespace DELTation.DIFramework.Editor
{
    [CustomEditor(typeof(Resolver))]
    internal sealed class ResolverCustomEditor : UnityEditor.Editor
    {
        private GUIStyle _headerStyle;
        private bool _foldout;
        private ResolverReport _report;

        private void OnEnable()
        {
            _headerStyle = new GUIStyle
            {
                normal = new GUIStyleState
                {
                    textColor = Color.white,
                },
                richText = true,
            };

            _report = new ResolverReport((Resolver) serializedObject.targetObject);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _report.Generate();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("<size=14><b>Dependencies</b></size>:", _headerStyle);
            GUILayout.FlexibleSpace();
            var resolutionText = GetResolutionText(_report);
            GUILayout.Box(resolutionText, _headerStyle);
            GUILayout.EndHorizontal();

            _foldout = EditorGUILayout.BeginFoldoutHeaderGroup(_foldout, "Components");

            if (_foldout)
                foreach (var componentData in _report.ComponentsData)
                {
                    DrawComponent(componentData);
                }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private static string GetResolutionText(ResolverReport report)
        {
            var (resolved, notResolved, notInjectable) = (report.Resolved, report.NotResolved, report.NotInjectable);

            return new StringBuilder()
                .Append(notResolved > 0 || notInjectable > 0 ? "<color=red>" : "<color=green>")
                .AppendFormat("<b>{0} resolved, {1} failed to resolve, {2} not injectable</b>", resolved.ToString(),
                    notResolved.ToString(), notInjectable.ToString()
                )
                .Append("</color>")
                .ToString();
        }

        private static void DrawComponent(ComponentResolutionData componentData)
        {
            var color = GUI.color;
            GUI.color = Color.white;

            GUILayout.BeginHorizontal();

            if (componentData.Dependencies.Length > 0)
            {
                const int indentPerLevel = 20;
                GUILayout.Space(componentData.Depth * indentPerLevel);
                GUILayout.Label("-", GUILayout.Width(10));
                DrawReadonlyField(componentData.Component);
                GUILayout.FlexibleSpace();
            }

            if (componentData.Injectable)
            {
                if (componentData.Dependencies.Length > 0)
                    DrawDependencies(componentData);
            }
            else
            {
                DrawDependencyBox("Not injectable");
            }

            GUILayout.EndHorizontal();

            GUI.color = color;
        }

        private static void DrawReadonlyField(MonoBehaviour component)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(component, typeof(Component), true, GUILayout.Width(200));
            EditorGUI.EndDisabledGroup();
        }

        private static void DrawDependencies(ComponentResolutionData componentData)
        {
            foreach (var (dependency, source) in componentData.Dependencies)
            {
                var dependencyText = dependency.Name;
                DrawDependencyBox(dependencyText, source);
            }
        }

        private static void DrawDependencyBox(string text, DependencySource? source = null)
        {
            GUI.color = source.HasValue ? Color.green : Color.red;

            StringBuilder.Clear().Append(text);
            if (source.HasValue)
            {
                var sourceIndicator = GetDependencySourceIndicator(source.Value);
                StringBuilder.Append(" (").Append(sourceIndicator).Append(")");
            }

            var content = new GUIContent(StringBuilder.ToString());
            if (source.HasValue)
                content.tooltip = source.Value.ToString();

            GUILayout.Box(content);
        }

        private static readonly StringBuilder StringBuilder = new StringBuilder();

        private static string GetDependencySourceIndicator(DependencySource source)
        {
            switch (source)
            {
                case DependencySource.Local:
                    return "L";
                case DependencySource.Children:
                    return "C";
                case DependencySource.Parent:
                    return "P";
                case DependencySource.Global:
                    return "G";
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }
    }
}