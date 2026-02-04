using System;
using _Project.Scripts.Player;
using UnityEngine;

namespace _Project.Scripts.Interactive
{
    public class InterItem : MonoBehaviour
    {
        [SerializeField] private Material extra;
        [SerializeField] private Material extraS;
        [SerializeField] private bool oneshot;
        
        private Material _selected;
        private Material _selectedS;
        private Material _def;

        private SpriteRenderer _sprite;
        private Animator _anim;

        public static InterItem Stayed;
        private static readonly int Interact = Animator.StringToHash("Interact");

        private bool _used;
        public event Action<InterItem> Used;

        [Header("Extra")]
        [SerializeField] private bool realFantasy;
        [SerializeField] private ItemView uiView;

        public static event Action doFantasyy;

        private void Start()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _anim = GetComponent<Animator>();
            
            _selected = Resources.Load<Material>("Outline");
            _selectedS = Resources.Load<Material>("OutlineS");
            _def = Resources.Load<Material>("Sprite-Lit-Default");
            if (extra != null) _selected = extra;
            if (extraS != null) _selectedS = extraS;
            
            _sprite.material = _selected;
        }
        
        private void Update()
        {
            if (_used) _selected = _def;
            if (_used) _selectedS = _def;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!enabled) return;
            
            if (!other.CompareTag("Player")) return;
            if (oneshot && _used) return;
            Stayed = this;
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!enabled) return;
            
            if (!other.CompareTag("Player")) return;
            _sprite.material = _selected;
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (!enabled) return;
            
            if (!other.CompareTag("Player")) return;
            if (oneshot && _used)
            {
                GetComponent<Collider2D>().enabled = false;
                enabled = false;
                return;
            }

            if (Stayed != this)
            {
                _sprite.material = _selected;
                return;
            }
            _sprite.material = _selectedS;
            
            if (!InteractButton.Instance.Interact) return;
            _anim.SetTrigger(Interact);
            _sprite.material = _def;
            _used = true;
            Used?.Invoke(this);
            FindAnyObjectByType<PlayerMovement>().ResetTarget();
            if (realFantasy) DoFantasy();
        }

        private void DoFantasy()
        {
            SoundManager.instance.PlayClip(Resources.Load<AudioClip>("RealFantasy"));
            doFantasyy?.Invoke();
        }

        public void KeySeeUI() => uiView.SeeView();
    }
}