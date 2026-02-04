using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _Project.Scripts.EntryPoints
{
    [ExecuteInEditMode]
    public class GameEntryPoint : MonoBehaviour
    {
        [SerializeField] private Light2D[] globals;
        private float _targetIntensity;
        
        [SerializeField, Space(5)] private List<SceneState> states;

        private int _lastOnStart = -100;

        private void Start()
        {
            _targetIntensity = globals[0].intensity;
            
            if (!Application.isPlaying) return;
            states.ForEach(s =>
            {
                if (s.onStart) SoundManager.instance.PlayClip(s.sound);
            });
        }

        private void Update()
        {
            if (!Application.isPlaying) return;
            ChangeLight(_targetIntensity);
        }
        
        public void ChangeLight(float intensity)
        {
            if (Mathf.Approximately(globals[0].intensity, intensity)) return;
            globals[0].intensity = Mathf.MoveTowards(globals[0].intensity, intensity, Time.deltaTime);
            globals[1].intensity = Mathf.MoveTowards(globals[1].intensity, intensity, Time.deltaTime);
        }
        
        private void OnValidate()
        {
            if (states.Count == 0 || states[0].scene == null)
            {
                states = new List<SceneState>();
                for (var i = 0; i < transform.childCount; i++) 
                    states.Add(new SceneState(transform.GetChild(i).gameObject, Resources.Load<AudioClip>(transform.GetChild(i).name)));
            }
            
            var allFalse = true;
            for (var i = 0; i < states.Count; i++)
            {
                if (!states[i].onStart) continue;
                allFalse = false;
                if (i == 0) _lastOnStart = 0;
                if (i == _lastOnStart || _lastOnStart == -100) continue;
                states[_lastOnStart].onStart = false;
                states[i].onStart = true;
                _lastOnStart = i;
            }
            if (allFalse)
            {
                states[0].onStart = true;
                _lastOnStart = 0;
            }
            
            states.ForEach(s => s.scene.SetActive(s.onStart));
        }
    }

    [System.Serializable]
    public class SceneState
    {
        [ReadOnly] public GameObject scene;
        [ReadOnly] public AudioClip sound;
        public bool onStart;

        public SceneState(GameObject scene, AudioClip sound)
        {
            this.scene = scene;
            this.sound = sound;
        }
    }
}
