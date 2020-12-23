using UnityEngine;

namespace DELTation.DIFramework.PlayTests.Components
{
	public class LoopComponent2 : MonoBehaviour
	{
		public LoopComponent1 Component { get; private set; }

		public void Construct(LoopComponent1 component)
		{
			Component = component;
		}
	}
}