using UnityEngine;

namespace PerformanceTest.Scripts.DI
{
	public class Dependent3 : MonoBehaviour
	{
		[SerializeField] private bool _resolveManually = false;

		private void Awake()
		{
			if (_resolveManually)
				Construct(GetComponentInChildren<Dependency3>());
		}

		public void Construct(Dependency3 dependency)
		{
			_dependency = dependency;
		}
		
		private Dependency3 _dependency;
	}
}