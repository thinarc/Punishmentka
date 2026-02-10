using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

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

        public CanvasGroup menu;
        public RectTransform menuRect;
        public CanvasGroup logo;
        public RectTransform logoRect;
        public RectTransform buttonRect;

        private void BeforeMenu()
        {
            upLayers.ForEach(g => g.SetActive(false));
            texture.sharedMaterial.SetFloat("_InkSpreadDistance", inkSpread);
            texture.sharedMaterial.SetFloat("_FadingFade", 1);
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
            texture.material = blur;
            
            logoRect.DOScale(0.6f, 0);
            menu.DOFade(0, 0);
            logo.DOFade(0, 0);
            menu.interactable = true;
            menu.blocksRaycasts = true;
            BeforeMenu();
            logoRect.DOScale(1, 0.64f).SetEase(Ease.OutBack);
            await logo.DOFade(1, 0.8f).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
            menu.DOFade(1, 0.44f).SetEase(Ease.InOutSine);
        }

        public async UniTask UndoMaterial()
        {
            await UniTask.WaitWhile(() =>
            {
                var val = texture.sharedMaterial.GetFloat("_FadingFade");
                texture.sharedMaterial.SetFloat("_FadingFade", 1);
                return val != 0;
            });
            await UniTask.Delay(5000);
        }

        public async void UndoMenu(Material start)
        {
            var tween = buttonRect.DOShakePosition(0.1f, 3f).SetLoops(-1).SetEase(Ease.OutBack);
            buttonRect.GetComponent<Button>().interactable = false;
            inkSpeadAnim = true;
            
            await UniTask.Delay(2000);
            logo.DOFade(0, 0.74f).SetEase(Ease.InOutSine);
            await UniTask.Delay(TimeSpan.FromSeconds(0.4f));
            menu.DOFade(0, 0.34f).SetEase(Ease.InOutSine);

            await UndoMaterial();
            texture.sharedMaterial = start;
            upLayers.ForEach(g => g.SetActive(true));
            
            studyButton.SetTrigger("Study");
        }
    }
}