using System;
using System.Collections.Generic;
using DELTation.DIFramework.Resolution;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Dependencies
{
    public interface IDependency
    {
        string GetInternalDependencyTypeName();

        [NotNull]
        object ProduceInitializedObject(PocoInjection.ResolutionFunction resolutionFunction);

        [NotNull, MustUseReturnValue]
        Type GetResultingType();

        void GetDependencies([NotNull] ICollection<Type> dependencies);
    }
}