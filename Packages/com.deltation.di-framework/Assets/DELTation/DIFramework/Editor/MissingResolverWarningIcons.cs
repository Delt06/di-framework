using DELTation.DIFramework.Resolution;
using UnityEditor;
using UnityEngine;

namespace DELTation.DIFramework.Editor
{
    [InitializeOnLoad]
    public static class MissingResolverWarningIcons
    {
        static MissingResolverWarningIcons() => EditorApplication.hierarchyWindowItemOnGUI += UpdateIcons;

        private static void UpdateIcons(int instanceId, Rect selectionRect)
        {
            if (Application.isPlaying) return;
            if (!DiSettings.TryGetInstance(out var settings) || !settings.ShowMissingResolverWarnings) return;

            if (!(EditorUtility.InstanceIDToObject(instanceId) is GameObject gameObject)) return;
            if (HasResolverInParent(gameObject)) return;
            if (!IsInjectable(gameObject)) return;

            DrawIcon(selectionRect);
        }

        private static bool HasResolverInParent(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out Resolver _))
                return true;

            var parent = gameObject.transform.parent;
            return parent != null && HasResolverInParent(parent.gameObject);
        }

        private static bool IsInjectable(GameObject gameObject)
        {
            var components = gameObject.GetComponents<MonoBehaviour>();

            foreach (var component in components)
            {
                if (component == null) continue;
                var type = component.GetType();
                var constructMethods = Injection.GetConstructMethods(type);
                if (constructMethods.Count > 0)
                    return true;
            }

            return false;
        }

        private static void DrawIcon(Rect selectionRect)
        {
            const float iconSize = 16f;
            var rect = new Rect(selectionRect.x + selectionRect.width - iconSize, selectionRect.y, iconSize, iconSize);
            var icon = LoadIcon();
            GUI.DrawTexture(rect, icon);
        }

        private static Texture2D LoadIcon() => AssetDatabase.LoadAssetAtPath<Texture2D>(
            "Packages/com.deltation.di-framework/Assets/DELTation/DIFramework/Editor/Graphics/resolver_warning.png"
        );
    }
}