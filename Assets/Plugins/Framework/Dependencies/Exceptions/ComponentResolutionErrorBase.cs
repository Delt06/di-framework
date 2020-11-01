using System;
using Object = UnityEngine.Object;

namespace Framework.Dependencies.Exceptions
{
	public abstract class ComponentResolutionErrorBase : Exception
	{
		protected ComponentResolutionErrorBase(string message) : base(message) { }

		protected static string FormatMessage(Object context, Type componentType) =>
			$"Component of type {componentType.Name} was not found in {context.name}";
	}
}