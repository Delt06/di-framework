using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime.Components
{
    public class LoopComponent1 : MonoBehaviour
    {
        public LoopComponent2 Component { get; private set; }

        public void Construct(LoopComponent2 component)
        {
            Component = component;
        }
    }
}