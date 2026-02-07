using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerAnim : MonoBehaviour
    {
        [SerializeField] private AnimatorOverrideController[] overrides;
        private RuntimeAnimatorController _controller;

        [SerializeField] private float idleMulti = 1;
        [SerializeField] private float walkMulti = 1;
        
        private Animator _anim;
        private PlayerMovement _player;

        [SerializeField] private RuntimeAnimatorController start;
        public int _current;

        private void Start()
        {
            _anim = GetComponent<Animator>();
            _player = GetComponentInParent<PlayerMovement>();
            
            if (start == null) _controller = _anim.runtimeAnimatorController;
            else _controller = start;
        }

        private void Update()
        {
            var velocity = _player.Velocity;
            _anim.SetFloat("idlemulti", idleMulti);
            _anim.SetBool("walk", velocity.magnitude > 0.03f);
            
            var acceleration = velocity.magnitude;
            if (acceleration < 0.24f) acceleration = 0.24f;
            acceleration *= 1.56f;
            if (acceleration > 0.99f) acceleration = 0.99f;
            _anim.SetFloat("walkmulti", walkMulti * acceleration);
            
            if (velocity.magnitude <= 0.03f) return;
            if (Mathf.Abs(_player.Velocity.x) > Mathf.Abs(_player.Velocity.y))
            {
                if (velocity.x > 0 && _current != 3) ChangeController(3);
                else if (velocity.x < 0 && _current != 2) ChangeController(2);
            }
            else if (Mathf.Abs(_player.Velocity.x) < Mathf.Abs(_player.Velocity.y))
            {
                if (velocity.y > 0 && _current != 1) ChangeController(1);
                else if (velocity.y < 0 && _current != 0) ChangeController(0);
            }
        }

        private void ChangeController(int? state = null)
        {
            if (state != null) _current = state.Value;
            var controller = state != 0 ? overrides[_current - 1] : _controller;
            
            if (_anim.runtimeAnimatorController == controller) return;
            _anim.runtimeAnimatorController = controller;
        }
    }
}
