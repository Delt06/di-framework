using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime.Components
{
	public class ConstructChecker : MonoBehaviour
	{
		public bool Constructed { get; private set; }

		public void Construct() => Constructed = true;
	}
}