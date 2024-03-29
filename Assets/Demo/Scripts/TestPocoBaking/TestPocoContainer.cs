﻿using DELTation.DIFramework;
using DELTation.DIFramework.Containers;

namespace Demo.Scripts.TestPocoBaking
{
    public class TestPocoContainer : DependencyContainerBase
    {
        protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
        {
            builder.Register<TestPoco>()
                .Register<TestPocoDependency>();
        }
    }
}