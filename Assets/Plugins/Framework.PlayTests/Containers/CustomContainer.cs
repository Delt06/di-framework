using Framework.Dependencies.Containers;
using Framework.PlayTests.Components;

namespace Framework.PlayTests.Containers
{
	public class CustomContainer : DependencyContainerBase
	{
		public const string String = "Some String";

		protected override void ComposeDependencies()
		{
			Register(String);
			Register<Ignored>();
			Register<Child>();
		}
	}
}