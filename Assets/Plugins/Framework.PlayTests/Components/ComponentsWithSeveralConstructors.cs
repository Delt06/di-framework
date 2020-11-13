using UnityEngine;

namespace Framework.PlayTests.Components
{
	public class ComponentsWithSeveralConstructors : MonoBehaviour
	{
		public bool FirstCalled { get; private set; }
		public bool SecondCalled { get; private set; }
		public void Construct(Parent p) => FirstCalled = true;
		public void Construct(Child c) => SecondCalled = true;
	}
}