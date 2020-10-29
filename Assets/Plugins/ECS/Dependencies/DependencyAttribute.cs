using System;

namespace ECS.Dependencies
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class DependencyAttribute : Attribute
	{
		public Source Source { get; } = Source.Global;
		
		public DependencyAttribute() { }

		public DependencyAttribute(Source source = Source.Global) => Source = source;
	}
}