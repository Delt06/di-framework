using UnityEngine;

namespace Framework.PlayTests
{
	public class RigidbodyComponent : MonoBehaviour
	{
		public Rigidbody Rigidbody { get; private set; }

		public void Construct(Rigidbody rigidbody)
		{
			Rigidbody = rigidbody;
		}
	}

	public class LoopComponent1 : MonoBehaviour
	{
		public LoopComponent2 Component { get; private set; }

		public void Construct(LoopComponent2 component)
		{
			Component = component;
		}
	}

	public class LoopComponent2 : MonoBehaviour
	{
		public LoopComponent1 Component { get; private set; }

		public void Construct(LoopComponent1 component)
		{
			Component = component;
		}
	}
}