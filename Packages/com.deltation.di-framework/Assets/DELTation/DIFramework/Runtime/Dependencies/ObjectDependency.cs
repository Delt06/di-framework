using System;
using System.Collections.Generic;
using DELTation.DIFramework.Resolution;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Dependencies
{
    public class ObjectDependency : IDependency
    {
        private readonly object _object;

        public ObjectDependency([NotNull] object o) => _object = o ?? throw new ArgumentNullException(nameof(o));

        public object ProduceInitializedObject(PocoInjection.ResolutionFunction resolutionFunction) => _object;
        public Type GetResultingType() => _object.GetType();
        public void GetDependencies(ICollection<Type> dependencies) { }
    }
}