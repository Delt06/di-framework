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
			Register<Ignored>();
			Register<Child>();
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

	public class Parent { }

	public class Child : Parent { }

	public class ParentDependencyComponent : MonoBehaviour
	{
		public Parent Parent { get; private set; }

		public void Construct(Parent parent)
		{
			Parent = parent;
		}
	}
}