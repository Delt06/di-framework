using System;
using JetBrains.Annotations;
using UnityEngine;

namespace DELTation.DIFramework.Exceptions
{
    internal class NotInjectableException : Exception
    {
        public readonly MonoBehaviour Component;
        public readonly string ConstructorName;

        public NotInjectableException([NotNull] MonoBehaviour component, [NotNull] string constructorName)
        {
            Component = component ? component : throw new ArgumentNullException(nameof(component));
            ConstructorName = constructorName ?? throw new ArgumentNullException(nameof(constructorName));
            Message = $"{component}'s {constructorName} method is not injectable.";
        }

        public override string Message { get; }
    }
}