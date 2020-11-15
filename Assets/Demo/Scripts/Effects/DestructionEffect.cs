using System;
using Demo.Scripts.Damage;
using UnityEngine;

namespace Demo.Scripts.Effects
{
	public sealed class DestructionEffect : MonoBehaviour
	{
		[SerializeField] private GameObject _effect = default;

		public void Construct(IDestroyable destroyable)
		{
			_destroyable = destroyable;
		}

		private void OnEnable()
		{
			_destroyable.OnPreDestroyed += _onDestroyed;
		}

		private void OnDisable()
		{
			_destroyable.OnPreDestroyed -= _onDestroyed;
		}

		private void Awake()
		{
			_onDestroyed = (sender, args) => { Instantiate(_effect, transform.position, transform.rotation); };
		}

		private EventHandler _onDestroyed;
		private IDestroyable _destroyable;
	}
}