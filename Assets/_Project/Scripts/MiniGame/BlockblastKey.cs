using UnityEngine;

namespace _Project.Scripts.MiniGame
{
    public class BlockblastKey : MonoBehaviour
    {
        [SerializeField] private BlockForm blockForm = BlockForm.Box;
        private Vector2Int _size;
        
        private Animator _anim;

        private void Start()
        {
            _anim = GetComponent<Animator>();

            switch (blockForm)
            {
                case BlockForm.Box:  
            }
        }
    }
    
    public enum BlockForm
    {
        // (0,0) (1,0)
        // (0,1) (1,1)
        Box,
        
        // (0,0) (1,0)
        IHorizontal,
        
        // (0,0)
        // (0,1)
        IVertical,
        
        // (0,0) (1,0)
        // (0,1)
        LUpLeft,
        
        // (0,0) (1,0)
        //       (1,1)
        LUpRight,
        
        //       (1,0)
        // (0,1) (1,1)
        LDownRight,
        
        // (0,0)
        // (0,1) (1,1)
        LDownLeft,
    }
}