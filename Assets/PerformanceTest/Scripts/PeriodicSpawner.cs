using Unity.Profiling;
using UnityEngine;

namespace PerformanceTest.Scripts
{
    public class PeriodicSpawner : MonoBehaviour
    {
        public GameObject Prefab = default;
        public float Period = 1f;
        public int Count = 50;

        private void Update()
        {
            _timeTillNextSpawn -= Time.deltaTime;
            if (_timeTillNextSpawn > 0f) return;

            for (var i = 0; i < Count; i++)
            {
                _preparePerfMarker.Begin();
                Instantiate(Prefab);
                _preparePerfMarker.End();
            }

            _timeTillNextSpawn = Period;
        }

        private float _timeTillNextSpawn;

        private static ProfilerMarker _preparePerfMarker = new ProfilerMarker("Instantiate And Inject");
    }
}