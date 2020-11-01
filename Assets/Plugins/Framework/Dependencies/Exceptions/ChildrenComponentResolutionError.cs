using System;
using Object = UnityEngine.Object;

namespace Framework.Dependencies.Exceptions
{
	internal sealed class ChildrenComponentResolutionError : ComponentResolutionErrorBase
	{
		public ChildrenComponentResolutionError(Object context, Type componentType) :
			base(FormatMessage(context, componentType) + " nor in its children.") { }
	}
}