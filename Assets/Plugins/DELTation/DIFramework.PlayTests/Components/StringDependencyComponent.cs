using UnityEngine;

namespace DELTation.DIFramework.PlayTests.Components
{
	public class StringDependencyComponent : MonoBehaviour
	{
		public string String;

		public void Construct(string @string)
		{
			String = @string;
		}
	}
}