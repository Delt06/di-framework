using JetBrains.Annotations;

namespace DELTation.DIFramework.Baking
{
    public delegate object PocoInstantiationFunction([NotNull] PocoResolutionFunction resolutionFunction);
}