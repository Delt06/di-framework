using System;
using JetBrains.Annotations;

namespace DELTation.DIFramework.Exceptions
{
    public class DependencyNotResolvedException : Exception
    {
        public readonly Type Type;

        public DependencyNotResolvedException([NotNull] Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Message = $"Did not resolve dependency of type {type}.";
        }

        public override string Message { get; }
    }
}