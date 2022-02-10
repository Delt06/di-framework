using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace DELTation.DIFramework
{
    public sealed class DiSettings : ScriptableObject
    {
        private static DiSettings _instance;
        [SerializeField] private DependencySource _defaultDependencySource = DependencySources.All;
        [SerializeField] private bool _showIconsInHierarchy = true;
        [SerializeField] private bool _showMissingResolverWarnings = true;
        [SerializeField] private bool _useBakedData = true;
        [SerializeField] private bool _bakeOnBuild;
        [SerializeField] private string _bakedAssembliesRegex = @"^Assembly-CSharp$";

        public DependencySource DefaultDependencySource => _defaultDependencySource;

        public bool ShowIconsInHierarchy => _showIconsInHierarchy;

        public bool ShowMissingResolverWarnings => _showMissingResolverWarnings;

        public bool UseBakedData => _useBakedData;

        public bool BakeOnBuild => _bakeOnBuild;

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

        private void OnValidate()
        {
            _bakedAssembliesRegex = _bakedAssembliesRegex?.Trim() ?? string.Empty;
        }

        public bool ShouldBeBaked([NotNull] Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            var assemblyName = assembly.GetName().Name;
            var match = Regex.Match(assemblyName, _bakedAssembliesRegex);
            return match.Success;
        }

        public static bool TryGetInstance(out DiSettings settings)
        {
            settings = Instance;
            return settings != null;
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

            if (!AssetDatabase.IsValidFolder(fullFolderName))
                AssetDatabase.CreateFolder(parentFolder, folder);

            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
#endif

            return settings;
        }
    }
}