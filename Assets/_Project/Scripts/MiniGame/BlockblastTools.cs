using System;
using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.MiniGame
{
    public class BlockblastTools : MonoBehaviour
    {
        [Header("Debug")]
        [ShowInInspector] private List<BlockblastKey> _keys;
        
        public void InitKeys(List<Sprite> sheet, int area)
        {
            if (area is > 12 or < 2) throw new Exception("Area out of range: " + area);
            _keys ??= GetComponentsInChildren<BlockblastKey>(true).ToList();
            var keysBox = _keys.Where(k => k.BlockForm == BlockForm.Box).ToList();
            var keysL = _keys.Where(k => k.BlockForm is BlockForm.LUpLeft or BlockForm.LUpRight or BlockForm.LDownRight or BlockForm.LDownLeft).ToList();
            var keysI = _keys.Where(k => k.BlockForm is BlockForm.IHorizontal or BlockForm.IVertical).ToList();

            var boxes = area / 4;
            area %= 4;
            print("boxes: " + boxes);
            print("area / 4: " + area);
            area %= 3;
            var iforms = area / 2;
            area %= 2;
            
            print("lforms: " + lforms);
            print("iforms: " + iforms);

            var count = boxes + lforms + iforms;
            if (count is > 3 or < 1) throw new Exception("Count out of range: " + count);

            for (; boxes > 0; boxes--)
            {
                var randBox = keysBox[Random.Range(0, keysBox.Count)];
                keysBox.Remove(randBox);
                
                if (TryGetAnchor(randBox, out var ax, out var ay))
                {
                    randBox.gameObject.SetActive(true);
                    
                    var localSheet = BuildSheetForKey(randBox, sheet, ax, ay);
                    randBox.Fill(localSheet);
                }
                else return;
            }
            
            for (; lforms > 0; lforms--)
            {
                var randL = keysL[Random.Range(0, keysL.Count)];
                keysL.Remove(randL);
                
                if (TryGetAnchor(randL, out var ax, out var ay))
                {
                    var localSheet = BuildSheetForKey(randL, sheet, ax, ay);
                    randL.Fill(localSheet);
                }
                else return;
            }
            
            for (; iforms > 0; iforms--)
            {
                var randI = keysI[Random.Range(0, keysI.Count)];
                keysI.Remove(randI);
                
                if (TryGetAnchor(randI, out var ax, out var ay))
                {
                    var localSheet = BuildSheetForKey(randI, sheet, ax, ay);
                    randI.Fill(localSheet);
                }
                else return;
            }
        }
        
        private static Sprite CalcSprite(List<Sprite> sheet, int x, int y) => sheet[y * 8 + x];
        
        private static Sprite[] BuildSheetForKey(BlockblastKey key, List<Sprite> sheet, int anchorX, int anchorY)
        {
            var cells = key.GetCells();
            var result = new Sprite[cells.Length];

            for (var i = 0; i < cells.Length; i++)
            {
                var px = anchorX + cells[i].x;
                var py = anchorY + cells[i].y;

                result[i] = CalcSprite(sheet, px, py);
            }
            return result;
        }
        
        private static bool TryGetAnchor(BlockblastKey key, out int ax, out int ay)
        {
            var cells = key.GetCells();

            var maxX = 0;
            var maxY = 0;
            
            foreach (var c in cells)
            {
                if (c.x > maxX) maxX = c.x;
                if (c.y > maxY) maxY = c.y;
            }

            var limitX = 8 - (maxX + 1);
            var limitY = 8 - (maxY + 1);

            if (limitX < 0 || limitY < 0)
            {
                ax = ay = 0;
                return false;
            }

            ax = Random.Range(0, limitX + 1);
            ay = Random.Range(0, limitY + 1);
            return true;
        }
    }
}