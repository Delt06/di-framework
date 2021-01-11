using UnityEngine;

namespace PerformanceTest.Scripts.DI
{
	public class Dependent2 : MonoBehaviour
	{
		[SerializeField] private bool _resolveManually = false;

		private void Awake()
		{
			if (_resolveManually)
				Construct(GetComponentInChildren<Dependency2>());
		}

		public void Construct(Dependency2 dependency)
		{
			_dependency = dependency;
		}
		
		private Dependency2 _dependency;
	}
}