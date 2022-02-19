using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DELTation.DIFramework.Resolution;
using UnityEditor;
using UnityEngine;
using static DELTation.DIFramework.Editor.Baking.InjectionBakingData;

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

            var bakedTypes = GetAllBakedTypes().ToArray();

            foreach (var bakedType in bakedTypes)
            {
                if (bakedType.Name.StartsWith("<"))
                    Debug.Log(bakedType.Name);
            }

            var className = GetClassName();
            var baker = new InjectionBaker(className, bakedTypes);

            var pathParts = GetFoldersHierarchy();
            var classPath = GetClassPath(pathParts, className);

            var classFolder = GetClassFolder(pathParts);
            Directory.CreateDirectory(classFolder);

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
                .Where(a => DiSettings.TryGetInstance(out var settings) && settings.ShouldBeBaked(a))
                .SelectMany(a => a.GetTypes())
                .Where(CanBeBaked)
                .Distinct();

        private static bool CanBeBaked(Type type) =>
            type.IsClass &&
            type.IsPublic &&
            !IsCompilerGenerated(type) &&
            (typeof(MonoBehaviour).IsAssignableFrom(type) &&
             HasAtLeastOneConstructMethod(type) &&
             Injection.IsInjectable(type)
             ||
             IsInjectablePoco(type));

        private static bool IsCompilerGenerated(Type type) =>
            type.Name.StartsWith("<") ||
            type.Name.Contains("`") ||
            type.GetCustomAttribute<CompilerGeneratedAttribute>() != null;

        private static bool IsInjectablePoco(Type type) =>
            PocoInjection.IsPoco(type) &&
            PocoInjection.IsInjectable(type) &&
            type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length > 0;

        private static bool HasAtLeastOneConstructMethod(Type type) =>
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

        private static string GetClassFolder(IEnumerable<string> pathParts)
        {
            var parts = pathParts
                .Prepend(Application.dataPath)
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