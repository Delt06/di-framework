using UnityEngine;

namespace Framework.Dependencies.Containers
{
	public interface IIgnoreByContainer { }

	public sealed class IgnoreByContainer : MonoBehaviour, IIgnoreByContainer { }
}