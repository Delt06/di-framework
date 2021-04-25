using UnityEngine;

namespace PerformanceTest.Scripts.DI
{
    public class Dependent1 : MonoBehaviour
    {
        [SerializeField] private bool _resolveManually = false;

        private void Awake()
        {
            if (_resolveManually)
                Construct(GetComponentInChildren<Dependency1>());
        }

        public void Construct(Dependency1 dependency1)
        {
            _dependency1 = dependency1;
        }

        private Dependency1 _dependency1;
    }
}