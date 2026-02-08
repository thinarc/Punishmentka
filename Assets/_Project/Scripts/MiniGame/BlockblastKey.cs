using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.MiniGame
{
    public class BlockblastKey : MonoBehaviour
    {
        [SerializeField] private BlockForm blockForm = BlockForm.Box;
        private Vector2Int[] _cells;
        
        public BlockForm BlockForm => blockForm;

        private Animator _anim;

        [Header("Debug")]
        [SerializeField, Space(5)] private List<Image> squares;

        public Vector2Int[] GetCells()
        {
            _cells = CalcForm(blockForm);
            return _cells;
        }

        public void Fill(Sprite[] sheet)
        {
            _anim = GetComponent<Animator>();
            
            var cells = GetComponentsInChildren<RectTransform>()[1];
            if (cells.name != "Cells") throw new Exception("Cells not found");
            var index = 0;
            
            squares = cells.GetComponentsInChildren<Image>().ToList();
            squares.ForEach(s =>
            {
                s.sprite = sheet[index];
                index++;
            });
        }

        private static Vector2Int[] CalcForm(BlockForm form)
        {
            return form switch
            {
                // (0,0) (1,0)
                // (0,1) (1,1)
                BlockForm.Box => new Vector2Int[] { new(0, 0), new(1, 0), new(0, 1), new(1, 1), },
                // (0,0) (1,0)
                BlockForm.IHorizontal => new Vector2Int[] { new(0, 0), new(1, 0), },
                // (0,0)
                // (0,1)
                BlockForm.IVertical => new Vector2Int[] { new(0, 0), new(0, 1), },
                // (0,0) (1,0)
                // (0,1)
                BlockForm.LUpLeft => new Vector2Int[] { new(0, 0), new(1, 0), new(0, 1), },
                // (0,0) (1,0)
                //       (1,1)
                BlockForm.LUpRight => new Vector2Int[] { new(0, 0), new(1, 0), new(1, 1), },
                //       (1,0)
                // (0,1) (1,1)
                BlockForm.LDownRight => new Vector2Int[] { new(1, 0), new(0, 1), new(1, 1), },
                // (0,0)
                // (0,1) (1,1)
                BlockForm.LDownLeft => new Vector2Int[] { new(0, 0), new(0, 1), new(1, 1), },
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    
    public enum BlockForm
    {
        Box,
        LUpLeft,
        LUpRight,
        LDownRight,
        LDownLeft,
        IHorizontal,
        IVertical
    }
}