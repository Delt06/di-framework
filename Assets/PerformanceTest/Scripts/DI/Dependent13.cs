using UnityEngine;

namespace PerformanceTest.Scripts.DI
{
    public class Dependent13 : MonoBehaviour
    {
        [SerializeField] private bool _resolveManually = false;

        private void Awake()
        {
            if (_resolveManually)
                Construct(GetComponentInChildren<Dependency1>(), GetComponentInChildren<Dependency3>());
        }

        public void Construct(Dependency1 dependency1, Dependency3 dependency3)
        {
            _dependency1 = dependency1;
            _dependency3 = dependency3;
        }

        private Dependency1 _dependency1;
        private Dependency3 _dependency3;
    }
}