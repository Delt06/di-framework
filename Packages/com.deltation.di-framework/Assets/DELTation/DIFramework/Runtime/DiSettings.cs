using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework
{
    public sealed class DiSettings : ScriptableObject
    {
        [SerializeField] private bool _showIconsInHierarchy = true;
        [SerializeField] private bool _useBakedData = true;
        [SerializeField] private string _bakedAssembliesRegex = @"^Assembly-CSharp$";

        public bool ShowIconsInHierarchy => _showIconsInHierarchy;

        public bool UseBakedData => _useBakedData;

        public bool ShouldBeBaked([NotNull] Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            var assemblyName = assembly.GetName().Name;
            var match = Regex.Match(assemblyName, _bakedAssembliesRegex);
            return match.Success;
        }

        private void OnValidate()
        {
            _bakedAssembliesRegex = _bakedAssembliesRegex?.Trim() ?? string.Empty;
        }

        public static bool TryGetInstance(out DiSettings settings)
        {
            settings = Instance;
            return settings != null;
        }

        private static DiSettings Instance
        {
            get
            {
                if (_instance) return _instance;

                _instance = LoadSettingsOrDefault();
                if (_instance) return _instance;

                _instance = CreateSettings();
                return _instance;
            }
        }

        private static DiSettings LoadSettingsOrDefault() => Resources.LoadAll<DiSettings>("").FirstOrDefault();

        private static DiSettings CreateSettings()
        {
            var settings = CreateInstance<DiSettings>();

#if UNITY_EDITOR
            const string parentFolder = "Assets";
            const string folder = "Resources";
            const string fullFolderName = parentFolder + "/" + folder;
            const string assetPath = fullFolderName + "/DI Settings.asset";

            if (!UnityEditor.AssetDatabase.IsValidFolder(fullFolderName))
                UnityEditor.AssetDatabase.CreateFolder(parentFolder, folder);

            UnityEditor.AssetDatabase.CreateAsset(settings, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();
#endif

            return settings;
        }

        private static DiSettings _instance;
    }
}