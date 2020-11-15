using System.Collections;
using UnityEngine;

namespace Demo.Scripts
{
	public sealed class Bullet : MonoBehaviour
	{
		[SerializeField, Min(0f)] private float _lifetime = 1f;

		[Min(0f)] public float Speed = 10f;

		private void Update()
		{
			var direction = _transform.rotation * Vector3.forward;
			var translation = Speed * Time.deltaTime * direction;
			transform.Translate(translation, Space.World);
		}

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(_lifetime);
			Destroy(gameObject);
		}

		private void Awake()
		{
			_transform = transform;
		}

		private Transform _transform;
	}
}