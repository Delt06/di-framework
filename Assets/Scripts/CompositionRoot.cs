using Framework.Dependencies;
using Framework.Dependencies.Containers;

public sealed class CompositionRoot : DependencyContainerBase
{
	protected override void ComposeDependencies(ContainerBuilder builder)
	{
		builder.Register<InputSource>();
	}
}