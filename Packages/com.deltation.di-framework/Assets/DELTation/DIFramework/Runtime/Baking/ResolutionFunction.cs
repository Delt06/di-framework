using System;
using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework.Baking
{
    public delegate object ResolutionFunction([NotNull] MonoBehaviour component, [NotNull] Type type);
}