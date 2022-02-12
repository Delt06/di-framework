using System;
using System.Collections.Generic;
using System.Linq;
using DELTation.DIFramework.Pooling;
using DELTation.DIFramework.Resolution;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Dependencies
{
    public class CompositeDependency : IDependency
    {
        public delegate void CombinationHandler([NotNull] object primaryObject,
            [NotNull] IReadOnlyList<object> secondaryObjects);

        private readonly CombinationHandler _combine;
        private readonly IDependency _primaryDependency;
        private readonly IDependency[] _secondaryDependencies;

        public CompositeDependency([NotNull] IDependency mainDependency,
            [NotNull] IEnumerable<IDependency> innerDependencies,
            [NotNull] CombinationHandler combine)
        {
            _primaryDependency = mainDependency ?? throw new ArgumentNullException(nameof(mainDependency));
            if (innerDependencies == null) throw new ArgumentNullException(nameof(innerDependencies));
            _secondaryDependencies = innerDependencies.ToArray();
            _combine = combine ?? throw new ArgumentNullException(nameof(combine));
        }

        public object ProduceInitializedObject(PocoInjection.ResolutionFunction resolutionFunction)
        {
            var primaryObject = _primaryDependency.ProduceInitializedObject(resolutionFunction);

            var secondaryObjects = ListPool<object>.Rent();

            foreach (var secondaryDependency in _secondaryDependencies)
            {
                var secondaryObject = secondaryDependency.ProduceInitializedObject(resolutionFunction);
                secondaryObjects.Add(secondaryObject);
            }

            _combine(primaryObject, secondaryObjects);

            ListPool<object>.Rent();
            return primaryObject;
        }

        public Type GetResultingType() => _primaryDependency.GetResultingType();

        public void GetDependencies(ICollection<Type> dependencies)
        {
            _primaryDependency.GetDependencies(dependencies);

            foreach (var secondaryDependency in _secondaryDependencies)
            {
                secondaryDependency.GetDependencies(dependencies);
            }
        }
    }
}