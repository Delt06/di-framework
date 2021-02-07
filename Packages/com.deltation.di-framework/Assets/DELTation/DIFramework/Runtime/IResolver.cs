using System;
using UnityEngine;

namespace DELTation.DIFramework
{
	internal interface IResolver
	{
		void Resolve();
		bool CabBeResolvedSafe(MonoBehaviour component, Type type);
	}
}