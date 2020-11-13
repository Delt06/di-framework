using UnityEngine;

namespace Framework.PlayTests.Components
{
	public class ConstructChecker : MonoBehaviour
	{
		public bool Constructed { get; private set; }

		public void Construct() => Constructed = true;
	}
}