using Framework.Dependencies.Containers;

public sealed class CompositionRoot : DependencyContainerBase
{
	protected override void ComposeDependencies()
	{
		Register<InputSource>();
	}
}