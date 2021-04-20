using UnityEngine;

namespace DELTation.DIFramework
{
    public sealed class DiSettings : ScriptableObject
    {
        [SerializeField] private bool _showIconsInHierarchy = true;

        public bool ShowIconsInHierarchy => _showIconsInHierarchy;

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

        private static DiSettings LoadSettingsOrDefault() => Resources.Load<DiSettings>(AssetPath);

        private static DiSettings CreateSettings()
        {
            var settings = CreateInstance<DiSettings>();

#if UNITY_EDITOR
            if (!UnityEditor.AssetDatabase.IsValidFolder(FullFolderName))
                UnityEditor.AssetDatabase.CreateFolder(ParentFolder, Folder);

            UnityEditor.AssetDatabase.CreateAsset(settings, AssetPath);
            UnityEditor.AssetDatabase.SaveAssets();
#endif

            return settings;
        }

        private static DiSettings _instance;
        private const string AssetPath = FullFolderName + "/DI Settings.asset";
        private const string FullFolderName = ParentFolder + "/" + Folder;
        private const string ParentFolder = "Assets";
        private const string Folder = "Resources";
    }
}