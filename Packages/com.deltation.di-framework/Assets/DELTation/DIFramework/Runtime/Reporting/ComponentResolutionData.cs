using System;
using UnityEngine;

namespace DELTation.DIFramework.Reporting
{
    public readonly struct ComponentResolutionData : IEquatable<ComponentResolutionData>
    {
        public readonly MonoBehaviour Component;
        public readonly int Depth;
        public readonly bool Injectable;
        public readonly (Type type, DependencySource? source)[] Dependencies;

        public ComponentResolutionData(MonoBehaviour component, int depth, bool injectable,
            (Type type, DependencySource? source)[] dependencies)
        {
            Component = component;
            Depth = depth;
            Injectable = injectable;
            Dependencies = dependencies;
        }

        public bool Equals(ComponentResolutionData other) => Equals(Component, other.Component) &&
                                                             Depth == other.Depth && Injectable == other.Injectable &&
                                                             Equals(Dependencies, other.Dependencies);

        public override bool Equals(object obj) => obj is ComponentResolutionData other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Component != null ? Component.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ Depth;
                hashCode = (hashCode * 397) ^ Injectable.GetHashCode();
                hashCode = (hashCode * 397) ^ (Dependencies != null ? Dependencies.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}