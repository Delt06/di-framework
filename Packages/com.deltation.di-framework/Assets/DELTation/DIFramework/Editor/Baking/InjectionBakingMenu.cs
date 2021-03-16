using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DELTation.DIFramework.Resolution;
using UnityEditor;
using UnityEngine;
using static DELTation.DIFramework.Editor.Baking.InjectionBakingData;
using static DELTation.DIFramework.Editor.Baking.InjectionBakingTypesUtils;

namespace DELTation.DIFramework.Editor.Baking
{
    internal static class InjectionBakingMenu
    {
        private const string BakingMenuName = MenuHelper.MenuName + "/Baking";

        [MenuItem(BakingMenuName + "/Clear Baked Data")]
        public static void Clear()
        {
            Clear(true);
        }

        public static void Clear(bool log)
        {
            var pathParts = GetFoldersHierarchy();
            var className = GetClassName();
            var path = GetClassPathInAssets(pathParts, className);
            var existingData = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));
            if (existingData != null)
            {
                AssetDatabase.DeleteAsset(path);

                if (log)
                    Debug.Log("Cleared baked DI data.");
            }
            else
            {
                if (log)
                    Debug.Log("There is not baked DI data.");
            }
        }

        [MenuItem(BakingMenuName + "/Bake")]
        public static void Bake()
        {
            Clear(false);

            var bakedTypes = GetAllBakedTypes();
            var className = GetClassName();
            var baker = new InjectionBaker(className, bakedTypes);

            var pathParts = GetFoldersHierarchy();
            var classPath = GetClassPath(pathParts, className);

            using (var writer = File.CreateText(classPath))
            {
                baker.BakeData(writer);
            }

            Debug.Log($"Baked DI data for {baker.BakedTypesCount} types.");

            var classPathInAssets = GetClassPathInAssets(pathParts, className);
            AssetDatabase.ImportAsset(classPathInAssets);
        }

        private static string GetClassName() => ClassName;

        private static IEnumerable<Type> GetAllBakedTypes() =>
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(CanBeBaked)
                .Distinct();

        private static bool CanBeBaked(Type type) =>
            typeof(MonoBehaviour).IsAssignableFrom(type) &&
            !GetFullyQualifiedName(type).StartsWith(nameof(DELTation)) &&
            HasAtLeastOneConstructor(type) &&
            Injection.IsInjectable(type);

        private static bool HasAtLeastOneConstructor(Type type) =>
            type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Count(m => m.Name == Injection.Constructor) > 0;

        private static string GetClassPath(IEnumerable<string> pathParts, string className)
        {
            var parts = pathParts
                .Prepend(Application.dataPath)
                .Append(className + ".cs")
                .ToArray();
            return Path.Combine(parts);
        }

        private static string GetClassPathInAssets(IEnumerable<string> pathParts, string className)
        {
            var parts = pathParts
                .Prepend("Assets")
                .Append(className + ".cs")
                .ToArray();
            return Path.Combine(parts);
        }
    }
}