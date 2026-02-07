using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;

namespace _Project.Scripts.EntryPoints
{
    public class MenuInterface : MonoBehaviour
    {
        public static MenuInterface instance;
        private void Awake() => instance = this;
        
        public MeshRenderer texture;
        public Material blur;
        public Animator studyButton;
        public GameObject[] upLayers;

        private float inkSpread = 15f;
        private bool inkSpeadAnim;

        private void BeforeMenu()
        {
            upLayers.ForEach(g => g.SetActive(false));
            texture.sharedMaterial.SetFloat("_InkSpreadDistance", inkSpread);
        }

        private void Update()
        {
            if (inkSpeadAnim)
            {
                inkSpread = Mathf.Lerp(inkSpread, -5f, 1.0012f - Mathf.Exp(-0.2f * Time.unscaledDeltaTime));
                texture.sharedMaterial.SetFloat("_InkSpreadDistance", inkSpread);
            }
        }

        public async void StartMenu()
        {
            Time.timeScale = 0;
            texture.material = blur;
            
            BeforeMenu();

            await UniTask.Delay(1000, DelayType.UnscaledDeltaTime);
            // inkSpeadAnim = true;
            
            await UniTask.Delay(1000, DelayType.UnscaledDeltaTime);

            // await UniTask.Delay(2000);
            // studyButton.SetTrigger("Study");
        }

        public async void UndoMenu(Material start)
        {
            Time.timeScale = 1;
            texture.sharedMaterial = start;
            upLayers.ForEach(g => g.SetActive(true)); // ???????????????????????????????????????????????
            // ???????????????????????????????????????????????????????
        }
    }
}