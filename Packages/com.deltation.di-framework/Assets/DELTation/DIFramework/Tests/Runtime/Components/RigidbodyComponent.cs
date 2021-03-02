using UnityEngine;

namespace DELTation.DIFramework.Tests.Runtime.Components
{
    public class RigidbodyComponent : MonoBehaviour
    {
        public Rigidbody Rigidbody { get; private set; }

        public void Construct(Rigidbody rigidbody)
        {
            Rigidbody = rigidbody;
        }
    }
}