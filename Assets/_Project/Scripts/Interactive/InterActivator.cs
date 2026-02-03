using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Interactive
{
    public class InterActivator : MonoBehaviour
    {
        [SerializeField] private List<InterItem> itemsToActivate;
        private List<InterItem> _itemsToActivate;

        private void Awake()
        {
            _itemsToActivate = new List<InterItem>();
            
            itemsToActivate.ForEach(item =>
            {
                _itemsToActivate.Add(item);
                item.Used += OnUsed;
            });
        }

        private void OnUsed(InterItem item)
        {
            item.Used -= OnUsed;
            itemsToActivate.Remove(item);

            if (itemsToActivate.Count != 0) return;
            GetComponent<InterItem>().enabled = true;
            GetComponent<Collider2D>().enabled = true;
            _itemsToActivate.ForEach(i =>
            {
                i.enabled = false;
                i.GetComponent<Collider2D>().enabled = false;
            });
        }

        private void OnDestroy() => itemsToActivate.ForEach(item => item.Used -= OnUsed);
    }
}