using UnityEngine;

namespace Demo.Scripts
{
	public interface IShootFrom
	{
		Vector3 Position { get; }
		Quaternion Rotation { get; }
	}
}