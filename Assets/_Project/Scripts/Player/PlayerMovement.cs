using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        private float _speed;
        private float _maxSpeed;
        
        [SerializeField, Space(5)] private Camera inputCam;
        [SerializeField] private Transform footPos;
        [SerializeField] private ParticleSystem targetParticle;
        
        private Rigidbody2D _rb;
        private Vector2 _input;
        
        private Vector2 _target;
        private bool _hasTarget;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();

            _maxSpeed = speed;
            speed -= 0.24f;
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
                return;

            if (!IsPointerInsideScreen())
                return;

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            var world = inputCam.ScreenToWorldPoint(Input.mousePosition);
            _target = new Vector2(world.x, world.y);
            targetParticle.transform.position = _target;
            targetParticle.Play();
            _hasTarget = true;
        }
        
        private static bool IsPointerInsideScreen()
        {
            var pos = Input.mousePosition;

            return pos.x >= 0 && pos.x <= Screen.width &&
                   pos.y >= 0 && pos.y <= Screen.height;
        }

        private void FixedUpdate()
        {
            if (!_hasTarget)
            {
                _rb.linearVelocity = Vector2.zero;
                return;
            }

            var current = new Vector2(footPos.position.x, footPos.position.y);
            var direction = (_target - current);
            _speed = speed * direction.magnitude * 1.44f;
            _speed = Mathf.Min(_speed, _maxSpeed);

            if (direction.sqrMagnitude < 0.01f)
            {
                ResetTarget();
                return;
            }

            direction.Normalize();
            _rb.linearVelocity = direction * _speed;
        }

        public void ResetTarget()
        {
            _rb.linearVelocity = Vector2.zero;
            _hasTarget = false;
        }
    }
}
