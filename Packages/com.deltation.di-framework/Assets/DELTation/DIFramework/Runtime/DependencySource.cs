using System;

namespace DELTation.DIFramework
{
	[Flags]
	internal enum DependencySource
	{
		Local = 1 << 1,
		Children = 1 << 2,
		Parent = 1 << 3,
		Global = 1 << 4
	}
}