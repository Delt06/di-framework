using DELTation.DIFramework.Containers;
using DELTation.DIFramework.Tests.Runtime.Components;

namespace DELTation.DIFramework.Tests.Runtime.Containers
{
	public class CustomContainer : DependencyContainerBase
	{
		public const string String = "Some String";

		protected override void ComposeDependencies(ContainerBuilder builder)
		{
			builder.Register(String)
				.Register<Ignored>()
				.Register<Child>();
		}
	}
}