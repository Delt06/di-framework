using UnityEngine;

namespace Demo.Scripts.TestPocoBaking
{
    public class TestPocoDependant : MonoBehaviour
    {
        public void Construct(TestPoco testPoco)
        {
            _testPoco = testPoco;
        }

        private TestPoco _testPoco;
    }
}