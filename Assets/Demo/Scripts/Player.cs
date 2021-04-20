using System;
using Demo.Scripts.Damage;
using Demo.Scripts.Shooting;
using UnityEngine;

namespace Demo.Scripts
{
    public class Player : MonoBehaviour, IShootingTarget, IHittable, IDestroyable
    {
        [SerializeField, Min(1)] private int _maxHealth = 5;

        bool IShootingTarget.IsActive => _isAlive;

        public Vector3 Position => transform.position;

        public void Hit()
        {
            if (!_isAlive) return;

            _health--;
            if (_health < 0)
                _health = 0;

            if (_health > 0)
                return;

            _isAlive = false;
            OnPreDestroyed?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
        }

        public event EventHandler OnPreDestroyed;

        private void Awake()
        {
            _health = _maxHealth;
        }

        private bool _isAlive = true;
        private int _health;
    }
}