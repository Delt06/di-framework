using DELTation.DIFramework.Containers;
using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime.Containers
{
    public class CtorInjectionContainer : DependencyContainerBase
    {
        protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
        {
            builder
                .Register<IndirectStringDependent>()
                .Register<StringDependent>()
                .Register("abc");
        }

        public class IndirectStringDependent
        {
            public IndirectStringDependent(StringDependent dependent) => Dependent = dependent;
            public StringDependent Dependent { get; }
        }

        public class StringDependent
        {
            public StringDependent(string s) => S = s;
            public string S { get; }
        }

        public class StringDependentComponent : MonoBehaviour
        {
            public void Construct(IndirectStringDependent dependent)
            {
                Dependent = dependent;
            }

            public IndirectStringDependent Dependent { get; private set; }
        }
    }
}