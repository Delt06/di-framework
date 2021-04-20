using System;

namespace DELTation.DIFramework
{
    [Flags]
    public enum DependencySource
    {
        Local = 1 << 1, // From this GameObject
        Children = 1 << 2, // From children of the Resolver
        Parent = 1 << 3, // From parents of the Resolver
        Global = 1 << 4, // From root containers
    }
}