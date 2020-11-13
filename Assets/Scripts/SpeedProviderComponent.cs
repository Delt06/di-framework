using UnityEngine;

public class SpeedProviderComponent : MonoBehaviour, ISpeedProvider
{
	[SerializeField] private float _speed = 1f;

	public float Speed => _speed;
}