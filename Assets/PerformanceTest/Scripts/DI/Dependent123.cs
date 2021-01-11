using UnityEngine;

namespace PerformanceTest.Scripts.DI
{
	public class Dependent123 : MonoBehaviour
	{
		[SerializeField] private bool _resolveManually = false;

		private void Awake()
		{
			if (_resolveManually)
				Construct(GetComponentInChildren<Dependency1>(), GetComponentInChildren<Dependency2>(), GetComponentInChildren<Dependency3>());
		}

		public void Construct(Dependency1 dependency1, Dependency2 dependency2, Dependency3 dependency3)
		{
			_dependency1 = dependency1;
			_dependency2 = dependency2;
			_dependency3 = dependency3;
		}
		
		private Dependency1 _dependency1;
		private Dependency2 _dependency2;
		private Dependency3 _dependency3;
	}
}