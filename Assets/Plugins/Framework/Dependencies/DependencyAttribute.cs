using System;

namespace Framework.Dependencies
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class DependencyAttribute : Attribute
	{
		public Source Source { get; }

		public DependencyAttribute(Source source) => Source = source;
	}
}