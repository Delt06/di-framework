using UnityEngine;

namespace PerformanceTest.Scripts.DI
{
	public class Dependent12 : MonoBehaviour
	{
		[SerializeField] private bool _resolveManually = false;

		private void Awake()
		{
			if (_resolveManually)
				Construct(GetComponentInChildren<Dependency1>(), GetComponentInChildren<Dependency2>());
		}

		public void Construct(Dependency1 dependency1, Dependency2 dependency2)
		{
			_dependency1 = dependency1;
			_dependency2 = dependency2;
		}
		
		private Dependency1 _dependency1;
		private Dependency2 _dependency2;
	}
}