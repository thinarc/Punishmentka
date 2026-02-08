using UnityEngine;

namespace _Project.Scripts.MiniGame
{
    public class BlockblastTools : MonoBehaviour
    {
        private BlockblastKey[] _keys;

        public void Initialize() => _keys = GetComponentsInChildren<BlockblastKey>(true);
        
        private void UpdateModel()
        {

        }

        private void UpdateView()
        {
            _keys[0].gameObject.SetActive(true);
        }

        public void ShowKeys()
        {
            UpdateModel();
            UpdateView();
        }
    }
}