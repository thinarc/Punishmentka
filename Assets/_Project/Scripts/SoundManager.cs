using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance;
        
        public AudioSource bg;
        public AudioSource sfx;

        private void Start() => instance = this;
        
        public void PlayClip(AudioClip clip)
        {
            var fade = 2f;
            if (bg.clip == null) fade = 0f;
            bg.DOFade(0, fade).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
            {
                bg.clip = clip;
                bg.Play();
                bg.DOFade(1, 2f).SetEase(Ease.Linear).SetUpdate(true);
            });
        }

        public void PlaySfx(AudioClip clip) => sfx.PlayOneShot(clip);
    }
}
