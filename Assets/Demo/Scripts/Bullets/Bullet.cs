using System;
using System.Collections;
using Demo.Scripts.Damage;
using UnityEngine;

namespace Demo.Scripts.Bullets
{
	public sealed class Bullet : MonoBehaviour, IDestroyable
	{
		[SerializeField, Min(0f)] private float _lifetime = 1f;

		[Min(0f)] public float Speed = 10f;

		public void Destroy()
		{
			OnPreDestroyed?.Invoke(this, EventArgs.Empty);
			Destroy(gameObject);
		}

		public event EventHandler OnPreDestroyed;

		private void Update()
		{
			var direction = _transform.rotation * Vector3.forward;
			var translation = Speed * Time.deltaTime * direction;
			transform.Translate(translation, Space.World);
		}

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(_lifetime);
			Destroy();
		}

		private void Awake()
		{
			_transform = transform;
		}

		private Transform _transform;
	}
}