using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

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

            await ShowProgress(true);
            
            var sprites = Resources.LoadAll<Sprite>("Grids/MonsterGrid");
            sprites.ForEach(s => sheet.Add(s));
            points = grid.GetComponentsInChildren<BlockblastKey>().ToList();
            var i = 0;
            points.ForEach(p =>
            {
                p.SetSprite(sprites[i]);
                i++;
            });
            
            while (true)
            {
                await ShowKeys();
                await ShowProgress();
            }
        }

        [SerializeField] private Slider progress;

        public async UniTask ShowProgress(bool start = false)
        {
            if (start) 
            {
                progress.gameObject.SetActive(false);
                return;
            }
            progress.gameObject.SetActive(true);
            progress.value = GetProgress();
            
            await UniTask.Delay(2000);
        }
        
        public float GetProgress()
        {
            int filled = 0;
            int width = _shape.GetLength(0);
            int height = _shape.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (_shape[x, y])
                        filled++;
                }
            }

            int total = width * height;

            return (float)filled / total;
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

            List<int> free = new();

            // собрать все свободные клетки
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (!_shape[x, y])
                        free.Add(x + y * 8);
                }
            }

            // перемешать
            for (int i = 0; i < free.Count; i++)
            {
                int rnd = Random.Range(i, free.Count);
                (free[i], free[rnd]) = (free[rnd], free[i]);
            }

            // взять первые addes
            int count = Mathf.Min(Random.Range(2, 10), free.Count);

            for (int i = 0; i < count; i++)
            {
                int index = free[i];
                int x = index % 8;
                int y = index / 8;

                _shape[x, y] = true;
                points[index].Show();
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
