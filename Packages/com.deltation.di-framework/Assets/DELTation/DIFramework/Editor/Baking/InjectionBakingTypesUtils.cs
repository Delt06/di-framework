using System;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Editor.Baking
{
    internal static  class InjectionBakingTypesUtils
    {
        public static string GetFullyQualifiedName([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return type.FullName?.Replace("+", ".") ?? string.Empty;
        }
    }
}