using JetBrains.Annotations;

namespace DELTation.DIFramework.Baking
{
    /// <summary>
    /// Represents a procedure to create an instance of POCO.
    /// </summary>
    public delegate object PocoInstantiationFunction([NotNull] PocoResolutionFunction resolutionFunction);
}