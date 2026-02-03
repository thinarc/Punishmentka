using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.EntryPoints
{
    [ExecuteInEditMode]
    public class GameEntryPoint : MonoBehaviour
    {
        [SerializeField] private List<SceneState> states;

        private int _lastOnStart = -100;
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

        private void Start()
        {
            if (!Application.isPlaying) return;
            states.ForEach(s =>
            {
                if (s.onStart) SoundManager.instance.PlayClip(s.sound);
            });
            
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
