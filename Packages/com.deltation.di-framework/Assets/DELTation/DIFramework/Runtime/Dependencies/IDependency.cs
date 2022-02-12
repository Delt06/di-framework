using System;
using System.Collections.Generic;
using DELTation.DIFramework.Resolution;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Dependencies
{
    internal interface IDependency
    {
        [NotNull]
        object GetOrCreateObject(PocoInjection.ResolutionFunction resolutionFunction);

        [NotNull, MustUseReturnValue]
        Type GetResultingType();

        void GetDependencies([NotNull] ICollection<Type> dependencies);
    }
}