using System;
using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework.Baking
{
    /// <summary>
    /// Represents a function to find component's dependency.
    /// </summary>
    public delegate object ResolutionFunction([NotNull] MonoBehaviour component, [NotNull] Type type);
}