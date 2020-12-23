using UnityEngine;

namespace DELTation.DIFramework.PlayTests.Components
{
	public class DefaultExecutionOrderScript : MonoBehaviour
	{
		public void Construct()
		{
			if (Awaken)
				AwakenWhenConstructed = true;
		}

		public bool AwakenWhenConstructed { get; private set; }
		public bool Awaken { get; private set; }

		private void Awake()
		{
			Awaken = true;
		}
	}
}