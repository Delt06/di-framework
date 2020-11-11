using Framework.Dependencies.Containers;
using UnityEngine;

namespace Framework.PlayTests
{
	public class CustomContainer : DependencyContainerBase
	{
		public const string String = "Some String";

		protected override void ComposeDependencies()
		{
			Register(String);
			Register(new Ignored());
		}
	}

	public class StringDependencyComponent : MonoBehaviour
	{
		public string String;

		public void Construct(string @string)
		{
			String = @string;
		}
	}

	public class Ignored : IIgnoreByContainer { }
}