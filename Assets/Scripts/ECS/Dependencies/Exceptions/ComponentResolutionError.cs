using System;
using Object = UnityEngine.Object;

namespace ECS.Dependencies.Exceptions
{
	public sealed class ComponentResolutionError : ComponentResolutionErrorBase
	{
		public ComponentResolutionError(Object context, Type componentType) :
			base(FormatMessage(context, componentType) + ".") { }
	}
}