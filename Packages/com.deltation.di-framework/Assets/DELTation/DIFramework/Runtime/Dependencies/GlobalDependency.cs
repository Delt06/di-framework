using System;
using System.Collections.Generic;
using DELTation.DIFramework.Resolution;

namespace DELTation.DIFramework.Dependencies
{
    internal class GlobalDependency : IDependency
    {
        private readonly Type _type;

        public GlobalDependency(Type type) => _type = type;

        public string GetInternalDependencyTypeName() => "Global";

        public object ProduceInitializedObject(PocoInjection.ResolutionFunction resolutionFunction) =>
            throw new InvalidOperationException("Global dependency cannot be instantiated.");

        public Type GetResultingType() => _type;

        public void GetDependencies(ICollection<Type> dependencies) { }
    }
}