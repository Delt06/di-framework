using System;
using UnityEngine;

namespace Framework.Reporting
{
	public readonly struct ComponentResolutionData
	{
		public readonly MonoBehaviour Component;
		public readonly int Depth;
		public readonly bool Injectable;
		public readonly (Type type, bool canBeResolved)[] Dependencies;

		public ComponentResolutionData(MonoBehaviour component, int depth, bool injectable,
			(Type type, bool canBeResolved)[] dependencies)
		{
			Component = component;
			Depth = depth;
			Injectable = injectable;
			Dependencies = dependencies;
		}
	}
}