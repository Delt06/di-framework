using System;
using System.Collections.Generic;
using DELTation.DIFramework.Resolution;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Dependencies
{
    public class TypeDependency : IDependency
    {
        private readonly Type _type;

        public TypeDependency([NotNull] Type type) => _type = type ?? throw new ArgumentNullException(nameof(type));

        public string GetInternalDependencyTypeName() => "Type";

        public object ProduceInitializedObject(PocoInjection.ResolutionFunction resolutionFunction) =>
            PocoInjection.CreateInstance(_type, resolutionFunction);

        public Type GetResultingType() => _type;

        public void GetDependencies(ICollection<Type> dependencies)
        {
            if (!PocoInjection.TryGetInjectableConstructorParameters(_type, out var parameters)) return;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < parameters.Count; index++)
            {
                var parameterInfo = parameters[index];
                dependencies.Add(parameterInfo.ParameterType);
            }
        }
    }
}