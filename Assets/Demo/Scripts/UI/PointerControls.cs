using Demo.Scripts.Movement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Demo.Scripts.UI
{
    public sealed class PointerControls : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler,
        IDragHandler
    {
        [SerializeField, Min(0f)] private float _deadzoneInViewport = 0.1f;

        public void Construct(Camera camera, IMovement movement)
        {
            _camera = camera;
            _movement = movement;
        }

        private void Update()
        {
            if (_pointerId.HasValue)
            {
                var offset = ToViewPort(_lastPosition) - ToViewPort(_initialPosition);
                offset = Vector3.ClampMagnitude(offset, 1f);

                if (offset.magnitude < _deadzoneInViewport)
                {
                    Stop();
                }
                else
                {
                    _movement.Horizontal = offset.x;
                    _movement.Vertical = offset.y;
                }
            }
            else
            {
                Stop();
            }
        }

        private Vector2 ToViewPort(Vector2 uiPoint) => _camera.ScreenToViewportPoint(uiPoint);

        private void Stop()
        {
            _movement.Horizontal = _movement.Vertical = 0f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_pointerId.HasValue) return;
            _pointerId = eventData.pointerId;
            _initialPosition = eventData.position;
            _lastPosition = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_pointerId != eventData.pointerId) return;
            _pointerId = null;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_pointerId != eventData.pointerId) return;
            _pointerId = null;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_pointerId != eventData.pointerId) return;
            _lastPosition = eventData.position;
        }

        private void OnDisable()
        {
            Stop();
        }

        private Camera _camera;
        private IMovement _movement;

        private int? _pointerId;
        private Vector2 _initialPosition;
        private Vector2 _lastPosition;
    }
}