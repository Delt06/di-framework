using System;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Exceptions
{
    internal class DependencyNotRegisteredException : Exception
    {
        public readonly Type Type;

        public DependencyNotRegisteredException([NotNull] Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Message = $"Dependency of type {type} is not registered.";
        }

        public override string Message { get; }
    }
}