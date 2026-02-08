using UnityEngine;

namespace _Project.Scripts.MiniGame
{
    public class Blockblast : MonoBehaviour
    {
        [SerializeField, Space(5)] private RectTransform source;
        [SerializeField] private float cellSize;
        
        [SerializeField, Space(5)] private BlockblastTools tools;
        
        private Animator _anim;

        private bool[,] _shape = new bool[8, 8];

        private void Start()
        {
            _anim = GetComponent<Animator>();

            RunGame();
        }

        public void RunGame()
        {
            tools.Initialize();
            print(Resources.LoadAll<Sprite>("Grids/MonsterGrid").Length);
            
            UpdateModel();
            UpdateView();
        }

        private void UpdateModel()
        {

        }

        private void UpdateView()
        {
            _anim.enabled = true;
            _anim.SetTrigger("Show");
            
            tools.ShowKeys();
        }

        private void OnDrawGizmos()
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
