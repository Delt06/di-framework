using UnityEngine;

namespace Demo.Scripts.Shooting
{
	public interface IShootFrom
	{
		Vector3 Position { get; }
		Quaternion Rotation { get; }
	}
}