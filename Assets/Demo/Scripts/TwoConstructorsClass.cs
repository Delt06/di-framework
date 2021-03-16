using UnityEngine;

namespace Demo.Scripts
{
    public class TwoConstructorsClass : MonoBehaviour
    {
        public void Construct(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
        }

        public void Construct(Rigidbody2D rigidbody2D)
        {
            _rigidbody2D = rigidbody2D;
        }
        
        private Rigidbody _rigidbody;
        private Rigidbody2D _rigidbody2D;
    }
}