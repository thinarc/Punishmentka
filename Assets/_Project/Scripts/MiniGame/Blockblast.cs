using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;

namespace _Project.Scripts.MiniGame
{
    public class Blockblast : MonoBehaviour
    {
        [SerializeField] private RectTransform source;
        [SerializeField] private RectTransform grid;
        [SerializeField, Space(5)] private float cellSize;
        
        [SerializeField, Space(5)] private BlockblastTools tools;
        
        private Animator _anim;

        [Header("Debug")]
        [SerializeField, Space(5)] private List<Sprite> sheet;
        [SerializeField] private List<BlockblastKey> points;
        private readonly bool[,] _shape = new bool[8, 8];

        private void Start()
        {
            _anim = GetComponent<Animator>();
            RunGame();
        }

        public async void RunGame()
        {
            _anim.enabled = true;
            _anim.SetTrigger("Show");
            
            var sprites = Resources.LoadAll<Sprite>("Grids/MonsterGrid");
            sprites.ForEach(s => sheet.Add(s));
            points = grid.GetComponentsInChildren<BlockblastKey>().ToList();
            var i = 0;
            points.ForEach(p =>
            {
                p.SetSprite(sprites[i]);
                i++;
            });

            while (Time.time < 25)
            {
                await ShowKeys();
            }
        }

        private int _cycle;
        public async UniTask ShowKeys()
        {
            _cycle++;
            var randSquare = Random.Range(5, 13);
            if (_cycle > 4) randSquare = Random.Range(1, 7);
            await tools.InitKeys(sheet, randSquare);
        }

        public bool TryGetCellPosition(Vector2 screenPos, Camera cam, out Vector2Int cell)
        {
            cell = default;
            
            screenPos.y += 92f;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(source, screenPos, cam, out var local)) return false;
            local.y *= -1;
            
            local.x += source.rect.width / 2;
            local.y += source.rect.height;
            print(new Vector2(local.x, local.y));

            var x = Mathf.FloorToInt(local.x * cellSize) - 1;
            var y = Mathf.FloorToInt(local.y * cellSize) - 1;

            if (x < 0 || y < 0 || x >= 8 || y >= 8)
                return false;

            cell = new Vector2Int(x, y);
            return true;
        }
        
        public bool TryPlace(Vector2Int origin, Vector2Int[] cells)
        {
            foreach (var c in cells)
            {
                var x = origin.x + c.x;
                if (x is < 0 or >= 8) x = origin.x - c.x;
                var y = origin.y + c.y;
                if (y is < 0 or >= 8) y = origin.y - c.y;
                
                if (x < 0 || y < 0 || x >= 8 || y >= 8)
                    return false;
                
                // if (_shape[x, y])
                //     return false;
            }

            foreach (var c in cells)
            {
                var x = origin.x + c.x;
                if (x is < 0 or >= 8) x = origin.x - c.x;
                var y = origin.y + c.y;
                if (y is < 0 or >= 8) y = origin.y - c.y;
                _shape[x, y] = true;
                points[x + y * 8].Show();
            }

            var addes = 6;
            for (var a = 0; a < _shape.GetLength(0); a++)
            {
                for (var b = 0; b < _shape.GetLength(1); b++)
                {
                    if (!_shape[a, b])
                    {
                        if (addes > 0 && Random.Range(0, 11) == 10)
                        {
                            _shape[a, b] = true;
                            points[a + b * 8].Show();
                            addes--;
                        }
                        else continue;
                    }
                }
            }
            
            return true;
        }
        
        public Vector3 GetWorldFromCell(Vector2Int cell)
        {
            var x = cell.x / cellSize;
            var y = 56 - cell.y / cellSize;

            var local = new Vector2(x, y);
            
            local.x -= source.rect.width * source.pivot.x;
            local.y -= source.rect.height * source.pivot.y;
            if (local.x < 0) local.x += 8;
            if (local.y < 0) local.y += 8;

            return source.TransformPoint(local);
        }
        
        private void OnDrawGizmosSelected()
        {
            var topLeft = new Vector3(source.rect.xMin, source.rect.yMax);

            for (var y = 0; y < 8; y++)
            {
                for (var x = 0; x < 8; x++)
                {
                    Gizmos.color = _shape[x, y] ? Color.red : Color.green;

                    var pos = topLeft;
                    pos += new Vector3(cellSize * 32, cellSize * -32);
                    pos += new Vector3(x * cellSize * 64, y * cellSize * -64);
                    pos = source.TransformPoint(pos);

                    Gizmos.DrawCube(pos, new Vector3(cellSize, cellSize));
                }
            }
        }

        public void BlockAnim() => _anim.enabled = false;
    }
}
