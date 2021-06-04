using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework.Baking
{
    /// <summary>
    /// Represents a procedure to satisfy component's dependencies.
    /// </summary>
    public delegate void InjectionFunction([NotNull] MonoBehaviour component,
        [NotNull] ResolutionFunction resolutionFunction);
}