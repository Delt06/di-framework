using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime.Components.Benchmark
{
    public class Component1 : MonoBehaviour
    {
        public void Construct() { }
    }

    public class Component2 : MonoBehaviour
    {
        public void Construct(Component1 component) { }
    }

    public class Component3 : MonoBehaviour
    {
        public void Construct(Component2 component) { }
    }

    public class Component4 : MonoBehaviour
    {
        public void Construct(Component3 component) { }
    }

    public class Component5 : MonoBehaviour
    {
        public void Construct(Component4 component) { }
    }

    public class Component6 : MonoBehaviour
    {
        public void Construct(Component5 component) { }
    }

    public class Component7 : MonoBehaviour
    {
        public void Construct(Component6 component) { }
    }

    public class Component8 : MonoBehaviour
    {
        public void Construct(Component7 component) { }
    }

    public class Component9 : MonoBehaviour
    {
        public void Construct(Component8 component) { }
    }

    public class Component10 : MonoBehaviour
    {
        public void Construct(Component9 component) { }
    }

    public class Component11 : MonoBehaviour
    {
        public void Construct(Component10 component) { }
    }

    public class Component12 : MonoBehaviour
    {
        public void Construct(Component11 component) { }
    }

    public class Component13 : MonoBehaviour
    {
        public void Construct(Component12 component) { }
    }

    public class Component14 : MonoBehaviour
    {
        public void Construct(Component13 component) { }
    }

    public class Component15 : MonoBehaviour
    {
        public void Construct(Component14 component) { }
    }

    public class Component16 : MonoBehaviour
    {
        public void Construct(Component15 component) { }
    }

    public class Component17 : MonoBehaviour
    {
        public void Construct(Component16 component) { }
    }

    public class Component18 : MonoBehaviour
    {
        public void Construct(Component17 component) { }
    }

    public class Component19 : MonoBehaviour
    {
        public void Construct(Component18 component) { }
    }

    public class Component20 : MonoBehaviour
    {
        public void Construct(Component19 component) { }
    }
}