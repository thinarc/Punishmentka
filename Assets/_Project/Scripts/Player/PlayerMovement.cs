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
        private bool _hasTarget;

        public Vector2 Velocity => _agent.velocity;

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _startSpeed = _agent.speed;
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

        public async void DoFreeze(float d)
        {
            _agent.speed = _startSpeed / 10f;
            await UniTask.Delay(TimeSpan.FromSeconds(d / 4));
            _agent.speed = _startSpeed / 7.4f;
            await UniTask.Delay(TimeSpan.FromSeconds(d / 4));
            _agent.speed = _startSpeed / 4f;
            await UniTask.Delay(TimeSpan.FromSeconds(d / 4));
            _agent.speed = _startSpeed / 2f;
            await UniTask.Delay(TimeSpan.FromSeconds(d / 4));
            _agent.speed = _startSpeed;
            
            ResetTarget();
        }

        public void ResetTarget() => _agent.ResetPath();
    }
}
