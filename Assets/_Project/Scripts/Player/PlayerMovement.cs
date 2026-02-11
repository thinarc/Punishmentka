using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace _Project.Scripts.Player
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField, Space(5)] private Camera inputCam;
        [SerializeField] private Transform footPos;
        [SerializeField] private ParticleSystem targetParticle;
        
        private NavMeshAgent _agent;
        private Vector2 _input;
        private float _startSpeed;
        
        private Vector2 _target;

        public Vector2 Velocity => GetVelocity();

        public bool disable;
        
        private Vector2 GetVelocity()
        {
            if (_agent == null) _agent = GetComponent<NavMeshAgent>();
            return _agent.velocity;
        }

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _startSpeed = _agent.speed;
        }

        public void SetTarget(Vector2 target)
        {
            _target = target;
            _agent.SetDestination(_target);
            targetParticle.transform.position = _target;
            targetParticle.Play();
        }

        private void Update()
        {
            if (disable) return;
            
            if (!Input.GetMouseButtonDown(0))
                return;

            if (!IsPointerInsideScreen())
                return;

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;
            
            var world = inputCam.ScreenToWorldPoint(Input.mousePosition);
            _target = new Vector2(world.x, world.y);
            _agent.SetDestination(_target);
            targetParticle.transform.position = _target;
            targetParticle.Play();
        }
        
        private static bool IsPointerInsideScreen()
        {
            var pos = Input.mousePosition;

            return pos.x >= 0 && pos.x <= Screen.width &&
                   pos.y >= 0 && pos.y <= Screen.height;
        }

        public void ResetTarget() => _agent.ResetPath();
    }
}
