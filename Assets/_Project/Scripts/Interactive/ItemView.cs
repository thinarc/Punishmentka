using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Interactive
{
    public class ItemView : MonoBehaviour
    {
        [Header("Extra")]
        [SerializeField] private Image secondItem;
        [SerializeField] private CanvasGroup buttonForActivateSecondItem;
        
        public void SeeView()
        {
            Time.timeScale = 0f;
            var g = GetComponent<CanvasGroup>();
            g.blocksRaycasts = true;
            g.interactable = true;
            g.DOFade(1f, 0.4f).SetEase(Ease.OutSine).SetUpdate(true);

            if (!secondItem) return;
            g.interactable = false;
            g.blocksRaycasts = false;
            buttonForActivateSecondItem.interactable = true;
            buttonForActivateSecondItem.blocksRaycasts = true;
            buttonForActivateSecondItem.DOFade(1f, 0.4f).SetEase(Ease.OutSine).SetUpdate(true);
            buttonForActivateSecondItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                g.interactable = true;
                g.blocksRaycasts = true;
                buttonForActivateSecondItem.interactable = false;
                buttonForActivateSecondItem.blocksRaycasts = false;
                buttonForActivateSecondItem.DOFade(0f, 0.4f).SetEase(Ease.InSine).SetUpdate(true).OnComplete(() =>
                {
                    secondItem.DOFade(1f, 0.4f).SetEase(Ease.OutSine).SetDelay(0.2f).SetUpdate(true);
                });
            });
        }

        public void UnseeView()
        {
            Time.timeScale = 1f;
            var g = GetComponent<CanvasGroup>();
            g.blocksRaycasts = false;
            g.interactable = false;
            g.DOFade(0f, 0.4f).SetEase(Ease.InSine);
            
            if (!secondItem) return;
            secondItem.DOFade(0f, 0.4f).SetEase(Ease.InSine);
        }
    }
}
