using UnityEngine;

namespace Framework.PlayTests.Components
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