using static DELTation.DIFramework.DependencySource;

namespace DELTation.DIFramework
{
    internal static class DependencySources
    {
        public static DependencySource All => Local | Children | Parent | Global;
    }
}