using UnityEngine;

namespace DELTation.DIFramework.PlayTests.Components
{
	public class ParentDependencyComponent : MonoBehaviour
	{
		public Parent Parent { get; private set; }

		public void Construct(Parent parent)
		{
			Parent = parent;
		}
	}
}