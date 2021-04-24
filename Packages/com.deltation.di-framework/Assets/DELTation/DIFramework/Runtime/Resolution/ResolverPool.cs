using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework.Resolution
{
    internal static class ResolverPool
    {
        public static CachedComponentResolver Rent([NotNull] GameObject gameObject,
            DependencySource dependencySource)
        {
            if (gameObject == null) throw new ArgumentNullException(nameof(gameObject));

            if (FreeResolvers.Count == 0)
                return new CachedComponentResolver(gameObject, dependencySource);

            var lastIndex = FreeResolvers.Count - 1;
            var resolver = FreeResolvers[lastIndex];
            FreeResolvers.RemoveAt(lastIndex);
            FreeResolversSet.Remove(resolver);
            resolver.Clear();
            resolver.GameObject = gameObject;
            resolver.DependencySource = dependencySource;
            return resolver;
        }

        public static void Return([NotNull] CachedComponentResolver resolver)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            resolver.Clear();
            if (FreeResolversSet.Contains(resolver)) return;

            FreeResolvers.Add(resolver);
            FreeResolversSet.Add(resolver);
        }

        private static readonly HashSet<CachedComponentResolver> FreeResolversSet =
            new HashSet<CachedComponentResolver>();

        private static readonly List<CachedComponentResolver> FreeResolvers = new List<CachedComponentResolver>();
    }
}