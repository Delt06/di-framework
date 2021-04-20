using UnityEditor;
using UnityEngine;

namespace DELTation.DIFramework.Editor
{
    [InitializeOnLoad]
    public static class HierarchyIcons
    {
        static HierarchyIcons() => EditorApplication.hierarchyWindowItemOnGUI += UpdateIcons;

        private static void UpdateIcons(int instanceId, Rect selectionRect)
        {
            if (!DiSettings.TryGetInstance(out var settings) || !settings.ShowIconsInHierarchy) return;

            if (!(EditorUtility.InstanceIDToObject(instanceId) is GameObject gameObject)) return;
            if (!gameObject.TryGetComponent(out IShowIconInHierarchy showIconInHierarchy)) return;
            if (!(showIconInHierarchy is Object @object)) return;

            DrawIcon(@object, selectionRect);
        }

        private static void DrawIcon(Object @object, Rect selectionRect)
        {
            const float iconSize = 16f;
            var rect = new Rect(selectionRect.x + selectionRect.width - iconSize, selectionRect.y, iconSize, iconSize);
            GUI.DrawTexture(rect, GetObjectIcon(@object));
        }

        private static Texture GetObjectIcon(Object @object) =>
            EditorGUIUtility.ObjectContent(@object, @object.GetType()).image;
    }
}