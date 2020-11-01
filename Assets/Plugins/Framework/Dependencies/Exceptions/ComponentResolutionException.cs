using System;
using Object = UnityEngine.Object;

namespace Framework.Dependencies.Exceptions
{
	public sealed class ComponentResolutionException : ComponentResolutionExceptionBase
	{
		public ComponentResolutionException(Object context, Type componentType) :
			base(FormatMessage(context, componentType) + ".") { }
	}
}