using DELTation.DIFramework;
using DELTation.DIFramework.Containers;
using DELTation.DIFramework.Tests.Runtime.Components;
using DELTation.DIFramework.Tests.Runtime.Pocos;

namespace Demo.Scripts.Dependencies
{
    public sealed class CompositionRoot : DependencyContainerBase
    {
        protected override void ComposeDependencies(ContainerBuilder builder)
        {
            // Compose your dependencies here:
            // builder.Register(new T());
            // builder.Register<T>();
        }
    }
}