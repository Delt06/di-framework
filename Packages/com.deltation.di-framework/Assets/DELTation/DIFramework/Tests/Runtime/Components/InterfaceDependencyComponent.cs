using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime.Components
{
	public interface IInterface { }

	public class InterfaceImplementation : MonoBehaviour, IInterface { }

	public class InterfaceDependencyComponent : MonoBehaviour
	{
		public IInterface Dependency { get; private set; }

		public void Construct(IInterface dependency) => Dependency = dependency;
	}
}