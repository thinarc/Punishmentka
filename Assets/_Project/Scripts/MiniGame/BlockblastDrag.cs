using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.MiniGame
{
    public class BlockblastDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private RectTransform _rect;
        private Canvas _canvas;
        private Animator _anim;
        
        private Vector3 _startPos;
        private Vector2Int[] _cells;

        private void Start()
        {
            _anim = GetComponent<Animator>();
            _rect = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPos = _rect.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rect.position += (Vector3)eventData.delta / _canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _rect.position = _startPos;
        }
    }
}