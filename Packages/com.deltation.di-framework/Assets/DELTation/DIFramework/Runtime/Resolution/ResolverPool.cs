using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework.Resolution
{
    internal static class ResolverPool
    {
        public static CachedComponentResolver Rent([NotNull] MonoBehaviour resolverComponent,
            DependencySource dependencySource, bool useBakedData)
        {
            if (resolverComponent == null) throw new ArgumentNullException(nameof(resolverComponent));

            if (FreeResolvers.Count == 0)
            {
                return new CachedComponentResolver(resolverComponent, dependencySource, useBakedData);
            }

            var lastIndex = FreeResolvers.Count - 1;
            var resolver = FreeResolvers[lastIndex];
            FreeResolvers.RemoveAt(lastIndex);
            FreeResolversSet.Remove(resolver);
            resolver.Clear();
            resolver.ResolverComponent = resolverComponent;
            resolver.DependencySource = dependencySource;
            resolver.UseBakedData = useBakedData;
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
        
        private static readonly HashSet<CachedComponentResolver> FreeResolversSet = new HashSet<CachedComponentResolver>();
        private static readonly List<CachedComponentResolver> FreeResolvers = new List<CachedComponentResolver>();
    }
}