using UnityEngine;

namespace Framework.PlayTests.Components
{
	public class RigidbodyComponent : MonoBehaviour
	{
		public Rigidbody Rigidbody { get; private set; }

		public void Construct(Rigidbody rigidbody)
		{
			Rigidbody = rigidbody;
		}
	}
}