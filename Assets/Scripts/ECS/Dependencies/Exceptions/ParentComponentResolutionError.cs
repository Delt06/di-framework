﻿using System;
using Object = UnityEngine.Object;

namespace ECS.Dependencies.Exceptions
{
	public sealed class ParentComponentResolutionError : ComponentResolutionErrorBase
	{
		public ParentComponentResolutionError(Object context, Type componentType) :
			base(FormatMessage(context, componentType) + " nor in its parent.") { }
	}
}