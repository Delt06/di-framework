using System;
using System.Collections.Generic;
using DELTation.DIFramework.Resolution;

namespace DELTation.DIFramework.Dependencies
{
    internal readonly struct DependencyWithMetadata : IDependency, IEquatable<DependencyWithMetadata>
    {
        private readonly IDependency _dependency;
        public readonly HashSet<Type> Tags;

        public DependencyWithMetadata(IDependency dependency)
        {
            _dependency = dependency;
            Tags = new HashSet<Type>();
        }

        public object ProduceInitializedObject(PocoInjection.ResolutionFunction resolutionFunction) =>
            _dependency.ProduceInitializedObject(resolutionFunction);

        public Type GetResultingType() => _dependency.GetResultingType();
        public void GetDependencies(ICollection<Type> dependencies) => _dependency.GetDependencies(dependencies);

        public bool Equals(DependencyWithMetadata other) =>
            Equals(_dependency, other._dependency) && Equals(Tags, other.Tags);

        public override bool Equals(object obj) => obj is DependencyWithMetadata other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_dependency != null ? _dependency.GetHashCode() : 0) * 397) ^
                       (Tags != null ? Tags.GetHashCode() : 0);
            }
        }
    }
}