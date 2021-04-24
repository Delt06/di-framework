using System;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Baking
{
    public delegate object PocoResolutionFunction([NotNull] Type type);
}