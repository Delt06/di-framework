using Framework.Containers;
using UnityEngine;

namespace Framework.PlayTests.Containers
{
	public class CtorInjectionContainer : DependencyContainerBase
	{
		protected override void ComposeDependencies(ContainerBuilder builder)
		{
			builder
				.Register<IndirectStringDependent>()
				.Register<StringDependent>()
				.Register("abc");
		}

		public class IndirectStringDependent
		{
			public StringDependent Dependent { get; }
			public IndirectStringDependent(StringDependent dependent) => Dependent = dependent;
		}

		public class StringDependent
		{
			public string S { get; }
			public StringDependent(string s) => S = s;
		}

		public class StringDependentComponent : MonoBehaviour
		{
			public IndirectStringDependent Dependent { get; private set; }

			public void Construct(IndirectStringDependent dependent)
			{
				Dependent = dependent;
			}
		}
	}
}