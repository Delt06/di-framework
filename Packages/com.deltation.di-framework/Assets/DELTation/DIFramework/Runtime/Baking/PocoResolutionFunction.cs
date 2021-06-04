using System;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Baking
{
    /// <summary>
    /// Represents a function to find dependencies of an instance of POCO.
    /// </summary>
    public delegate object PocoResolutionFunction([NotNull] Type type);
}