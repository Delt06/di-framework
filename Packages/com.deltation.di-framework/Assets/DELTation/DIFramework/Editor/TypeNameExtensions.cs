using System;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Editor
{
    internal static class TypeNameExtensions
    {
        public static string GetFriendlyName([NotNull] this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var friendlyName = type.Name;
            if (!type.IsGenericType) return friendlyName;

            var backtickIndex = friendlyName.IndexOf('`');
            if (backtickIndex != -1) friendlyName = friendlyName.Remove(backtickIndex);
            friendlyName += "<";
            var typeArguments = type.GetGenericArguments();
            for (var i = 0; i < typeArguments.Length; ++i)
            {
                var argumentTypeName = GetFriendlyName(typeArguments[i]);
                friendlyName += i == 0 ? argumentTypeName : "," + argumentTypeName;
            }

            friendlyName += ">";

            return friendlyName;
        }
    }
}