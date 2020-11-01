using System;
using Object = UnityEngine.Object;

namespace Framework.Dependencies.Exceptions
{
	public sealed class ParentComponentResolutionError : ComponentResolutionErrorBase
	{
		public ParentComponentResolutionError(Object context, Type componentType) :
			base(FormatMessage(context, componentType) + " nor in its parent.") { }
	}
}