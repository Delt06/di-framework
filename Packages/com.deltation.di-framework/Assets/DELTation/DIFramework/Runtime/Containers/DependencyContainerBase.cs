using System;
using UnityEngine;

namespace DELTation.DIFramework.Containers
{
    public abstract class DependencyContainerBase : MonoBehaviour, IDependencyContainer
    {
        public bool TryResolve(Type type, out object dependency)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return InnerContainer.TryResolve(type, out dependency);
        }

        public bool CanBeResolvedSafe(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return InnerContainer.CanBeResolvedSafe(type);
        }

        protected abstract void ComposeDependencies(ContainerBuilder builder);

        private ConfigurableDependencyContainer InnerContainer => _innerContainer ??
                                                                  (_innerContainer =
                                                                      new ConfigurableDependencyContainer(
                                                                          ComposeDependencies
                                                                      ));

        private ConfigurableDependencyContainer _innerContainer;
    }
}