using System;
using System.Collections.Generic;
using System.Reflection;
using DELTation.DIFramework.Exceptions;
using DELTation.DIFramework.Resolution;
using JetBrains.Annotations;

namespace DELTation.DIFramework
{
    public readonly struct FactoryMethodDelegate : IEquatable<FactoryMethodDelegate>
    {
        private readonly Delegate _sourceDelegate;

        public readonly IReadOnlyList<ParameterInfo> ParameterTypes;
        public readonly bool IsValid;
        public readonly Type ReturnType;

        internal FactoryMethodDelegate([NotNull] Delegate sourceDelegate)
        {
            _sourceDelegate = sourceDelegate ?? throw new ArgumentNullException(nameof(sourceDelegate));
            var method = sourceDelegate.Method;
            ParameterTypes = method.GetParameters();
            if (!ParameterTypes.AreInjectable())
                throw new ArgumentException("Parameters of the source delegate are not injectable.",
                    nameof(sourceDelegate)
                );
            ReturnType = method.ReturnType;
            if (ReturnType == typeof(void))
                throw new ArgumentException("Return type should be non-void.",
                    nameof(sourceDelegate)
                );
            if (ReturnType.IsValueType)
                throw new ArgumentException("Return type should be a reference type (was value type instead).",
                    nameof(sourceDelegate)
                );
            IsValid = true;
        }

        public static implicit operator FactoryMethodDelegate(Delegate sourceDelegate) =>
            new FactoryMethodDelegate(sourceDelegate);

        public bool Equals(FactoryMethodDelegate other) => Equals(_sourceDelegate, other._sourceDelegate) &&
                                                           IsValid == other.IsValid && ReturnType == other.ReturnType;

        public override bool Equals(object obj) => obj is FactoryMethodDelegate other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _sourceDelegate != null ? _sourceDelegate.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ IsValid.GetHashCode();
                hashCode = (hashCode * 397) ^ (ReturnType != null ? ReturnType.GetHashCode() : 0);
                return hashCode;
            }
        }

        [NotNull]
        internal object Instantiate([NotNull] PocoInjection.ResolutionFunction resolutionFunction)
        {
            if (!IsValid) throw new InvalidOperationException("Factory method delegate is not valid.");
            if (resolutionFunction == null) throw new ArgumentNullException(nameof(resolutionFunction));

            var arguments = Injection.RentArgumentsArray(ParameterTypes.Count);

            for (var index = 0; index < ParameterTypes.Count; index++)
            {
                var parameterType = ParameterTypes[index].ParameterType;
                if (resolutionFunction(parameterType, out var dependency))
                {
                    arguments[index] = dependency;
                }
                else
                {
                    Injection.ReturnArgumentsArray(arguments);
                    throw new DependencyNotRegisteredException(parameterType);
                }
            }

            var instance = _sourceDelegate.DynamicInvoke(arguments);
            Injection.ReturnArgumentsArray(arguments);
            return instance;
        }
    }
}