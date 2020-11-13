using UnityEngine;

namespace Framework.PlayTests.Components
{
	public class LoopComponent1 : MonoBehaviour
	{
		public LoopComponent2 Component { get; private set; }

		public void Construct(LoopComponent2 component)
		{
			Component = component;
		}
	}
}