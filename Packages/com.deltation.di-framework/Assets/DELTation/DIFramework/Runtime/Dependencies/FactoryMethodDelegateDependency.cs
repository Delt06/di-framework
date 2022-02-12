using System;
using System.Collections.Generic;
using DELTation.DIFramework.Resolution;

namespace DELTation.DIFramework.Dependencies
{
    public class FactoryMethodDelegateDependency : IDependency
    {
        private readonly FactoryMethodDelegate _factoryMethodDelegate;

        public FactoryMethodDelegateDependency(FactoryMethodDelegate factoryMethodDelegate) =>
            _factoryMethodDelegate = factoryMethodDelegate;

        public object ProduceInitializedObject(PocoInjection.ResolutionFunction resolutionFunction) =>
            _factoryMethodDelegate.Instantiate(resolutionFunction);

        public Type GetResultingType() => _factoryMethodDelegate.ReturnType;

        public void GetDependencies(ICollection<Type> dependencies)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < _factoryMethodDelegate.ParameterTypes.Count; index++)
            {
                var parameterInfo = _factoryMethodDelegate.ParameterTypes[index];
                dependencies.Add(parameterInfo.ParameterType);
            }
        }
    }
}