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

        private void Start()
        {
            Stayed = null;
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
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (oneshot && _used) return;
            Stayed = this;
            _sprite.material = _selectedS;
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _sprite.material = _selected;
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (oneshot && _used) return;

            if (Stayed != this || _sprite.material == _selectedS)
            {
                _sprite.material = _selected;
                return;
            }
            
            _anim.ResetTrigger(Interact);
            if (!Input.GetKey(KeyCode.F)) return;
            _anim.SetTrigger(Interact);
            _sprite.material = _def;
            Stayed = null;
            _used = true;
        }
    }
}