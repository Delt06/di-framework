using DELTation.DIFramework.Containers;
using DELTation.DIFramework.PlayTests.Components;

namespace DELTation.DIFramework.PlayTests.Containers
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