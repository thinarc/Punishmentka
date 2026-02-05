using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace _Project.Scripts.EntryPoints
{
    [ExecuteInEditMode]
    public class GameEntryPoint : MonoBehaviour
    {
        [SerializeField] private List<SceneState> states;
        
        [SerializeField, Space(5)] private Light2D[] globals;
        private float _targetIntensity;

        [SerializeField, Space(5)] private VolumeProfile[] profiles;
        [SerializeField] private Volume[] volumeCamera;

        private const string pixelCameraName = "Pixel Camera";

        [SerializeField, Space(5)] private MeshRenderer worldView;
        [SerializeField] private MeshRenderer worldViewRep;
        [SerializeField] private Material[] renderMaterials;
        [SerializeField] private Transform worldViewData;
        [SerializeField] private Transform worldViewSmallData;

        [SerializeField, Space(5)] private SimpleConfiner2D confiner;

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
        
        public async void ChangeLight(float intensity)
        {
            _targetIntensity = intensity;
            if (Mathf.Approximately(globals[0].intensity, intensity)) return;
            globals[0].intensity = Mathf.MoveTowards(globals[0].intensity, intensity, Time.deltaTime / 40f);
            globals[1].intensity = Mathf.MoveTowards(globals[1].intensity, intensity, Time.deltaTime / 40f);
            await UniTask.Delay(3200);
            volumeCamera[0].profile = profiles[2];
        }
        
        private void ChangeScene()
        {
            states.ForEach(s =>
            {
                s.scene.SetActive(false);
                if (!s.onStart) return;

                s.scene.SetActive(true);
                confiner.boundsCollider = s.scene.GetComponent<PolygonCollider2D>();
                var vol = s.scene.name switch
                {
                    "Home" => 0,
                    "Fantasy" => 1,
                    "Final" => 3,
                    _ => -100
                };
                var isChangedView = s.scene.name switch
                {
                    "Home" => false,
                    "Fantasy" => false,
                    "Final" => true,
                    _ => false
                };
                var changeView = !isChangedView && volumeCamera[0].name != pixelCameraName || isChangedView && volumeCamera[0].name == pixelCameraName;
                if (changeView) (volumeCamera[0], volumeCamera[1]) = (volumeCamera[1], volumeCamera[0]);
                if (!volumeCamera[0].gameObject.activeInHierarchy) volumeCamera[0].gameObject.SetActive(true);
                if (volumeCamera[1].gameObject.activeInHierarchy) volumeCamera[1].gameObject.SetActive(false);
                volumeCamera[0].profile = profiles[vol];
                volumeCamera[1].profile = null;
                var renderMaterialsIndex = 0;
                if (isChangedView) renderMaterialsIndex = 2;
                if (worldView.sharedMaterial == renderMaterials[renderMaterialsIndex]) return;
                worldView.sharedMaterial = renderMaterials[renderMaterialsIndex];
                worldViewRep.sharedMaterial = renderMaterials[renderMaterialsIndex + 1];
                var scale = worldViewData.localScale;
                var pos = worldViewData.position;
                if (isChangedView) scale = worldViewSmallData.localScale;
                if (isChangedView) pos = worldViewSmallData.position;
                if (worldView.transform.localScale != scale) worldView.transform.localScale = scale;
                if (worldView.transform.position != pos) worldView.transform.position = pos;
                var lens = worldViewData.GetComponent<WorldViewLens>().lens;
                if (isChangedView) lens = worldViewSmallData.GetComponent<WorldViewLens>().lens;
                var allCam = confiner.GetComponent<Camera>();
                if (!Mathf.Approximately(allCam.orthographicSize, lens)) allCam.orthographicSize = lens;
            });
        }
        
#if UNITY_EDITOR
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
            
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null) ChangeScene();
            };
        }
#endif
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
