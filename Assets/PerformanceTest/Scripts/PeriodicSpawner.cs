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
				Instantiate(Prefab);
			}
			
			_timeTillNextSpawn = Period;
		}

		private float _timeTillNextSpawn;
	}
}