using System;
using UnityEngine;

namespace DELTation.DIFramework.Resolution
{
    internal readonly struct ResolutionContext : IEquatable<ResolutionContext>
    {
        public readonly GameObject Resolver;
        public readonly Component Component;

        public ResolutionContext(GameObject resolver, Component component)
        {
            Resolver = resolver;
            Component = component;
        }

        public bool Equals(ResolutionContext other) =>
            Equals(Resolver, other.Resolver) && Equals(Component, other.Component);

        public override bool Equals(object obj) => obj is ResolutionContext other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Resolver != null ? Resolver.GetHashCode() : 0) * 397) ^
                       (Component != null ? Component.GetHashCode() : 0);
            }
        }
    }
}