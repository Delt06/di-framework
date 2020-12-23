using UnityEngine;

namespace DELTation.DIFramework.PlayTests.Components
{
	public sealed class ResolutionCounter : MonoBehaviour
	{
		public int Count { get; private set; }

		public void Construct() => Count++;
	}
}