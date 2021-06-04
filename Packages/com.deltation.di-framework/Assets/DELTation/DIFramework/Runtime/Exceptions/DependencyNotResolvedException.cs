using System;
using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework.Exceptions
{
    public class DependencyNotResolvedException : Exception
    {
        public readonly Type Type;
        public readonly MonoBehaviour Component;

        public DependencyNotResolvedException([NotNull] MonoBehaviour component, [NotNull] Type type)
        {
            Component = component ? component : throw new ArgumentNullException(nameof(component));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Message = $"{component}: did not resolve dependency of type {type}.";
        }

        public override string Message { get; }
    }
}