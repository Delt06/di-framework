using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DELTation.DIFramework.Baking;
using DELTation.DIFramework.Resolution;
using UnityEditor;
using UnityEngine;
using static DELTation.DIFramework.Editor.MenuHelper;

namespace DELTation.DIFramework.Editor.Baking
{
    internal static class BakedInjectionDataGenerator
    {
        [MenuItem(MenuName + "/Baking/Bake")]
        public static void Bake()  // TODO: fix inconsistent line endings
        {
            Clear();
            
            int typesCount;

            using (var writer = File.CreateText(GetClassPath()))
            {
                WriteData(writer, out typesCount);
            }

            Debug.Log($"Baked DI data for {typesCount} types.");
        }

        private static void WriteData(StreamWriter writer, out int typesCount)
        {
            var index = 0;
            var bakeCalls = new StringBuilder();
            var injectionFunctionDeclaration = new StringBuilder();

            foreach (var type in GetInjectableTypes())
            {
                bakeCalls.Append(GetBakeCallText(type, index));
                injectionFunctionDeclaration.Append(GetInjectionFunctionDeclaration(type, index));
                injectionFunctionDeclaration.AppendLine();
                index++;
            }
            
            var dataText = string.Format(ClassTemplate, GetNamespace(), ClassName, bakeCalls, injectionFunctionDeclaration);
            writer.Write(dataText);
            typesCount = index;
        }

        private static IEnumerable<Type> GetInjectableTypes() =>
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(IsInjectable)
                .Distinct();

        private static bool IsInjectable(Type type)
        {
            return typeof(MonoBehaviour).IsAssignableFrom(type) &&
                   !GetFullyQualifiedName(type).StartsWith(nameof(DELTation)) && 
                   type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                       .Count(m => m.Name == Injection.Constructor) > 0 &&
                   Injection.IsInjectable(type);
        }

        [MenuItem(MenuName + "/Baking/Clear Baked Data")]
        public static void Clear()
        {
            var classPath = GetClassPath();
            if (File.Exists(classPath))
            {
                File.Delete(classPath);

                var metaFilePath = classPath + ".meta";
                if (File.Exists(metaFilePath))
                    File.Delete(metaFilePath);

                Debug.Log("Cleared baked DI data.");
            }
            else
            {
                Debug.Log("There is not baked DI data.");
            }
        }

        private static string GetClassPath()
        {
            var parts = PathParts
                .Prepend(Application.dataPath)
                .Append(ClassName + ".cs")
                .ToArray();
            return Path.Combine(parts);
        }

        private static string GetNamespace() => string.Join(".", PathParts.Skip(1));

        private static readonly string[] PathParts = { "Scripts", "DI", "Baked" };

        private const string ClassName = "BakedInjectionData";

        private const string ClassTemplate = @"// Generated automatically by DI Framework

using DELTation.DIFramework.Baking;
using UnityEngine;

namespace {0}
{{
    /// <summary>
    /// Baked injection data
    /// </summary>
    internal static class {1}
    {{
        [RuntimeInitializeOnLoadMethod]
        public static void Bake()
        {{
{2}
        }}

{3}
    }}
}}
";

        private static string GetBakeCallText(Type type, int injectionFunctionIndex)
        {
            var methodName = $"{nameof(BakedInjection)}.{nameof(BakedInjection.Bake)}";
            var fullTypeName = GetFullyQualifiedName(type);
            var typeName = $"typeof({fullTypeName})";
            var injectionFunctionName = GetInjectionFunctionName(injectionFunctionIndex);

            return new StringBuilder()
                .Append(TripleIndent)
                .Append(methodName)
                .Append("(")
                .Append(typeName)
                .Append(", ")
                .Append(injectionFunctionName)
                .AppendLine(");")
                .ToString();
        }

        private static string GetFullyQualifiedName(Type type) => type.FullName?.Replace("+", ".") ?? string.Empty;

        private static string GetInjectionFunctionName(int injectionFunctionIndex)
        {
            return $"InjectionFunction_{injectionFunctionIndex}";
        }

        private static string GetInjectionFunctionDeclaration(Type type, int injectionFunctionIndex)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(DoubleIndent)
                .Append("private static void ")
                .Append(GetInjectionFunctionName(injectionFunctionIndex))
                .Append($"({nameof(MonoBehaviour)} component, {nameof(ResolutionFunction)} resolve)")
                .AppendLine();

            stringBuilder.Append(DoubleIndent)
                .AppendLine("{");

            stringBuilder.Append(TripleIndent)
                .AppendLine($"var obj = ({GetFullyQualifiedName(type)}) component;");
            // TODO: implement injection calls


            stringBuilder.Append(DoubleIndent)
                .AppendLine("}");

            return stringBuilder.ToString();
        }
        
        private static readonly string DoubleIndent = new string(' ', SpacesInIndent * 2);
        private static readonly string TripleIndent = new string(' ', SpacesInIndent * 3);
        private const int SpacesInIndent = 4;
    }
}