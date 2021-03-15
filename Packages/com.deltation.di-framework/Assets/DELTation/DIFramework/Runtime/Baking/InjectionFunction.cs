using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework.Baking
{
    public delegate void InjectionFunction([NotNull] MonoBehaviour component, [NotNull] ResolutionFunction resolutionFunction);
}