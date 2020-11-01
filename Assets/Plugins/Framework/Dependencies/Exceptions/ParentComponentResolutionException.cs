using System;
using Object = UnityEngine.Object;

namespace Framework.Dependencies.Exceptions
{
	public sealed class ParentComponentResolutionException : ComponentResolutionExceptionBase
	{
		public ParentComponentResolutionException(Object context, Type componentType) :
			base(FormatMessage(context, componentType) + " nor in its parent.") { }
	}
}