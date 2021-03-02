using UnityEngine;

namespace DELTation.DIFramework.Resolution
{
    internal readonly struct ResolutionContext
    {
        public readonly Component Resolver;
        public readonly Component Component;

        public ResolutionContext(Component resolver, Component component)
        {
            Resolver = resolver;
            Component = component;
        }
    }
}