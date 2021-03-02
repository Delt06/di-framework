using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime.Components
{
    public class ComponentsWithSeveralConstructors : MonoBehaviour
    {
        public bool FirstCalled { get; private set; }
        public bool SecondCalled { get; private set; }
        public void Construct(Parent p) => FirstCalled = true;
        public void Construct(Child c) => SecondCalled = true;
    }
}