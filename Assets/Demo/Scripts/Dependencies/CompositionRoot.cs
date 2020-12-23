﻿using Framework;
using Framework.Containers;

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