using Framework.Dependencies;
using UnityEngine;

namespace Framework.Core
{
	public abstract class ComponentBase : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this.ResolveDependencies();
		}
	}
}