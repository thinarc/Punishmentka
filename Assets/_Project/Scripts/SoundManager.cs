using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance;
        
        public AudioSource bg;
        public AudioSource mind;
        public AudioSource sfx;

        private void Start() => instance = this;
        
        public void PauseMusic()
        {
            bg.Pause();
            sfx.Pause();
        }

        public void ResumeMusic()
        {
            bg.Play();
            sfx.Play();
        }

        public AudioClip pastHome;
        public async void PlayMind(AudioClip clip)
        {
            const float fade = 2f;
            bg.DOFade(0, fade).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
            {
                bg.Pause();
            });
            await UniTask.Delay(TimeSpan.FromSeconds(fade) / 2);
            if (mind.clip == null) mind.clip = clip;
            if (mind.loop == false) mind.loop = true;
            mind.DOFade(0, 0);
            mind.Play();
            mind.DOFade(1, fade).SetEase(Ease.Linear).SetUpdate(true);
        }
        
        public async void ReturnMind()
        {
            const float fade = 2f;
            mind.DOFade(0, fade).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
            {
                mind.Pause();
            });
            await UniTask.Delay(TimeSpan.FromSeconds(fade) / 2);
            bg.DOFade(0, 0);
            bg.Play();
            bg.DOFade(1, fade).SetEase(Ease.Linear).SetUpdate(true);
        }
        
        public async void PlayClip(AudioClip clip)
        {
            if (returned)
            {
                pastClip = clip;
                return;
            }
            
            var fade = 2.12f;
            if (bg.clip == null) fade = 0f;
            await bg.DOFade(0, fade).SetEase(Ease.Linear).SetUpdate(true).AsyncWaitForCompletion();
            bg.clip = clip;
            bg.Play();
            bg.DOFade(1, 2.72f).SetEase(Ease.Linear).SetUpdate(true);
        }

        public bool manual;
        public async void PlayClipManual(AudioClip clip)
        {
            manual = true;
            var fade = 2.12f;
            if (bg.clip == null)
            {
                fade = 0f;
                print("Fade out none??");
            }
            await bg.DOFade(0, fade).SetEase(Ease.Linear).SetUpdate(true).AsyncWaitForCompletion();
            bg.clip = clip;
            bg.Play();
            bg.DOFade(1, 2.72f).SetEase(Ease.Linear).SetUpdate(true);
        }

        public AudioClip pastClip;
        public bool returned;
        public async void ReturnPlayClip(AudioClip clip)
        {
            var fade = 2.12f;
            if (bg.clip == null) fade = 0f;
            await bg.DOFade(0, fade).SetEase(Ease.Linear).SetUpdate(true).AsyncWaitForCompletion();
            pastClip = bg.clip;
            bg.clip = clip;
            bg.loop = false;
            bg.Play();
            bg.DOFade(1, 2f).SetEase(Ease.Linear).SetUpdate(true);
            returned = true;
            
            await UniTask.WaitWhile(() => bg.isPlaying);
            if (manual) return;
            bg.clip = pastClip;
            bg.loop = true;
            await bg.DOFade(0, 0.12f).SetEase(Ease.Linear).SetUpdate(true).AsyncWaitForCompletion();
            bg.Play();
            bg.DOFade(1, 2.24f).SetEase(Ease.Linear).SetUpdate(true);
            returned = false;
        }

        public async UniTask PrepareFade()
        {
            const float fade = 2.12f;
            await bg.DOFade(0.4f, fade).SetEase(Ease.Linear).SetUpdate(true).AsyncWaitForCompletion();
            await sfx.DOFade(0, 0.36f).SetEase(Ease.Linear).SetUpdate(true).AsyncWaitForCompletion();
        }
        
        public void ReturnFade()
        {
            const float fade = 2.12f;
            bg.DOFade(1, fade).SetEase(Ease.Linear).SetUpdate(true);
        }
        
        public void PlayAfterPrepare(AudioClip clip)
        {
            sfx.clip = clip;
            sfx.loop = false;
            sfx.Play();
            sfx.DOFade(1, 1f).SetEase(Ease.Linear).SetUpdate(true);
        }

        public void PlaySfx(AudioClip clip) => sfx.PlayOneShot(clip);
    }
}
