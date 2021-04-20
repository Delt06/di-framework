using System;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Exceptions
{
    public class DependencyAlreadyRegistered : Exception
    {
        public readonly Type Type;
        public readonly object RegisteredDependency;

        public DependencyAlreadyRegistered([NotNull] Type type, [NotNull] object registeredDependency)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            RegisteredDependency =
                registeredDependency ?? throw new ArgumentNullException(nameof(registeredDependency));
            Message = $"Dependency of type {type} is already registered: {registeredDependency}.";
        }

        public override string Message { get; }
    }
}