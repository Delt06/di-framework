﻿using DELTation.DIFramework.Containers;

namespace DELTation.DIFramework.Tests.Runtime.Pocos
{
    public class SingleInstanceContainer<T> : DependencyContainerBase where T : class
    {
        protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
        {
            builder.Register<T>();
        }
    }
}