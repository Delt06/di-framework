using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DELTation.DIFramework.Editor
{
    [CustomEditor(typeof(RootDependencyContainer))]
    internal sealed class RootDependencyContainerCustomEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            _types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .SelectMany(a => a.GetExportedTypes())
                .Where(FilterTypes)
                .Prepend(null)
                .ToArray();

            _typeNames = _types
                .Select(t => t?.Name ?? "<Add Container>")
                .ToArray();
        }

        private static bool FilterTypes(Type type) =>
            typeof(IDependencyContainer).IsAssignableFrom(type) &&
            typeof(MonoBehaviour).IsAssignableFrom(type) &&
            type != typeof(RootDependencyContainer) &&
            !type.IsAbstract;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Attach other containers: ", EditorStyles.boldLabel);

            var selected = EditorGUILayout.Popup(0, _typeNames);
            if (selected != 0)
            {
                var container = (RootDependencyContainer) serializedObject.targetObject;
                container.gameObject.AddComponent(_types[selected]);
                EditorUtility.SetDirty(container.gameObject);
            }

            EditorGUILayout.EndHorizontal();
        }

        private string[] _typeNames = Array.Empty<string>();
        private Type[] _types = Array.Empty<Type>();
    }
}