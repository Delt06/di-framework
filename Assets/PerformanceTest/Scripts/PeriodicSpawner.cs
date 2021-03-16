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

			for (int i = 0; i < Count; i++)
			{
                PreparePerfMarker.Begin();
				Instantiate(Prefab);
                PreparePerfMarker.End();
			}
			
			_timeTillNextSpawn = Period;
		}

		private float _timeTillNextSpawn;

        private static readonly ProfilerMarker PreparePerfMarker = new ProfilerMarker("Instantiate And Inject");
	}
}