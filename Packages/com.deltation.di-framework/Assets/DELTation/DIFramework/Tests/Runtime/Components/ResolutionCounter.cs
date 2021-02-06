using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime.Components
{
	public sealed class ResolutionCounter : MonoBehaviour
	{
		public int Count { get; private set; }

		public void Construct() => Count++;
	}
}