using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Pincode
{
    public class CalledPin : MonoBehaviour
    {
        public CanvasGroup bg;
        public CanvasGroup pin;

        private void Start()
        {
            bg.alpha = 0;
            bg.interactable = false;
            bg.blocksRaycasts = false;
            pin.alpha = 0;
            pin.interactable = false;
            pin.blocksRaycasts = false;
            pin.gameObject.SetActive(false);
        }
        
        public async UniTask CheckPin()
        {
            bg.interactable = true;
            bg.blocksRaycasts = true;
            await bg.DOFade(1, 0.4f).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
            pin.gameObject.SetActive(true);
            pin.interactable = true;
            pin.blocksRaycasts = true;
            await pin.DOFade(1, 0.32f).SetEase(Ease.InBack).AsyncWaitForCompletion();

            var input = pin.GetComponent<TMP_InputField>();

            while (true)
            {
                // await input.OnEndEditAsync();
                await UniTask.WaitUntil(() => input.text.Length >= 4);
                if (input.text == "2792")
                {
                    input.text = "";
                    await UniTask.NextFrame();
                    input.text = "2792";
                    break;
                }
                input.text = "";
                input.interactable = false;
                await UniTask.Delay(100);
                input.interactable = true;
            }

            pin.DOFade(0, 0.54f).SetEase(Ease.OutBack);
            await UniTask.Delay(TimeSpan.FromSeconds(0.7f));
            bg.DOFade(0, 0.24f).SetEase(Ease.InOutSine);
            await UniTask.Delay(TimeSpan.FromSeconds(0.36f));
        }
    }
}
