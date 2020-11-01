using System;
using Object = UnityEngine.Object;

namespace Framework.Dependencies.Exceptions
{
	internal sealed class ChildrenComponentResolutionException : ComponentResolutionExceptionBase
	{
		public ChildrenComponentResolutionException(Object context, Type componentType) :
			base(FormatMessage(context, componentType) + " nor in its children.") { }
	}
}