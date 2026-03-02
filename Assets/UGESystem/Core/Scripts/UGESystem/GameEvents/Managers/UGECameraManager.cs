using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Cinemachine;

namespace UGESystem
{
    /// <summary>
    /// Manager component that controls all Cinemachine-based camera operations during events,
    /// such as camera switching, zooming, and shaking, according to commands.
    /// </summary>
    public class UGECameraManager : MonoBehaviour
    {
        [System.Serializable]
        private struct CameraStateSnapshot
        {
            public CinemachineVirtualCameraBase Camera;
            public int Priority;
            public float FieldOfView;
            public bool IsValid;
        }

        [Header("References")]
        [Tooltip("Assign the CinemachineBrain component in the scene.")]
        [SerializeField] private CinemachineBrain _brain;

        [Tooltip("Assign a default NoiseSettings profile (e.g., 6D Shake) for the Perlin Noise shake effect.")]
        [SerializeField] private NoiseSettings _defaultNoiseProfile;

        // The camera that was active BEFORE the event started
        private CameraStateSnapshot _goldenCameraState;
        
        // Dictionary to keep track of original states of ANY camera we modify during the event
        private Dictionary<CinemachineVirtualCameraBase, (int priority, float fov)> _modifiedCameras = new Dictionary<CinemachineVirtualCameraBase, (int, float)>();

        private Coroutine _activeShakeCoroutine;
        private Dictionary<string, CinemachineVirtualCameraBase> _sceneCameraCache = new Dictionary<string, CinemachineVirtualCameraBase>();

        private const int EVENT_CAM_PRIORITY_HIGH = 30;
        private const int DEFAULT_GAMEPLAY_CAM_PRIORITY = 10;
        private const int EVENT_CAM_PRIORITY_LOW = 0;

        private void Awake()
        {
            if (_brain == null) _brain = FindFirstObjectByType<CinemachineBrain>();
            CacheSceneCameras();
        }

        private void CacheSceneCameras()
        {
            _sceneCameraCache.Clear();
            var allCameras = FindObjectsByType<CinemachineVirtualCameraBase>(FindObjectsSortMode.None);
            foreach (var cam in allCameras)
            {
                if (!_sceneCameraCache.ContainsKey(cam.gameObject.name))
                    _sceneCameraCache.Add(cam.gameObject.name, cam);
            }
        }

        private CinemachineVirtualCameraBase FindCameraByName(string cameraName)
        {
            if (string.IsNullOrEmpty(cameraName)) return null;
            if (_sceneCameraCache.TryGetValue(cameraName, out var cam)) return cam;
            return null;
        }

        private void SaveCameraState(CinemachineVirtualCameraBase cam)
        {
            if (cam == null || _modifiedCameras.ContainsKey(cam)) return;
            
            float fov = 60f;
            if (cam is CinemachineCamera vcam) fov = vcam.Lens.FieldOfView;
            
            _modifiedCameras.Add(cam, (cam.Priority, fov));
        }

        /// <summary>
        /// Captures the initial state of the camera system before any event command runs.
        /// Only captures if no state is currently saved.
        /// </summary>
        public void PrepareForEvent()
        {
            // --- CRITICAL: Idempotency Check ---
            // 이미 골든 카메라가 저장되어 있다면 (이벤트 시퀀스 진행 중), 덮어쓰지 않습니다.
            if (_goldenCameraState.IsValid) return;

            if (_brain == null) return;
            
            var activeCam = _brain.ActiveVirtualCamera as CinemachineVirtualCameraBase;
            if (activeCam != null)
            {
                float fov = 60f;
                if (activeCam is CinemachineCamera vcam) fov = vcam.Lens.FieldOfView;

                _goldenCameraState = new CameraStateSnapshot
                {
                    Camera = activeCam,
                    Priority = activeCam.Priority,
                    FieldOfView = fov,
                    IsValid = true
                };
                
                SaveCameraState(activeCam);
            }
        }

        public IEnumerator SwitchTo(string cameraName, float blendDuration)
        {
            CinemachineVirtualCameraBase targetCam = FindCameraByName(cameraName);
            if (_brain == null || targetCam == null) yield break;

            SaveCameraState(targetCam);

            foreach (var cam in _modifiedCameras.Keys)
            {
                cam.Priority = EVENT_CAM_PRIORITY_LOW;
            }
            
            targetCam.Priority = EVENT_CAM_PRIORITY_HIGH;

            if (blendDuration > 0) yield return new WaitForSeconds(blendDuration);
        }

        public IEnumerator Zoom(string cameraName, float fov, float duration)
        {
            CinemachineVirtualCameraBase camToZoom = FindCameraByName(cameraName);
            if (camToZoom == null) camToZoom = _brain.ActiveVirtualCamera as CinemachineVirtualCameraBase;
            if (camToZoom == null || !(camToZoom is CinemachineCamera vcam)) yield break;

            SaveCameraState(camToZoom);

            float startFOV = vcam.Lens.FieldOfView;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                vcam.Lens.FieldOfView = Mathf.Lerp(startFOV, fov, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            vcam.Lens.FieldOfView = fov;
        }

        public void Shake(float intensity, float duration = 0.5f, float frequency = 1.0f)
        {
            if (_brain == null) return;
            var activeCam = _brain.ActiveVirtualCamera as CinemachineCamera;
            if (activeCam == null) return;

            SaveCameraState(activeCam);
            if (_activeShakeCoroutine != null) StopCoroutine(_activeShakeCoroutine);
            _activeShakeCoroutine = StartCoroutine(ShakeCoroutine(activeCam, intensity, duration, frequency));
        }

        private IEnumerator ShakeCoroutine(CinemachineCamera vcam, float intensity, float duration, float frequency)
        {
            var perlin = vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();
            if (perlin == null) perlin = vcam.gameObject.AddComponent<CinemachineBasicMultiChannelPerlin>();

            if (perlin.NoiseProfile == null && _defaultNoiseProfile != null)
                perlin.NoiseProfile = _defaultNoiseProfile;

            float elapsedTime = 0f;
            perlin.FrequencyGain = frequency;

            while (elapsedTime < duration)
            {
                perlin.AmplitudeGain = Mathf.Lerp(intensity, 0f, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            perlin.AmplitudeGain = 0f;
            _activeShakeCoroutine = null;
        }

        /// <summary>
        /// Nuclear Reset: Restores ALL modified cameras to their original states and forces the Golden Camera to be live.
        /// </summary>
        public void ResetCamera()
        {
            if (_brain == null) return;

            if (_activeShakeCoroutine != null)
            {
                StopCoroutine(_activeShakeCoroutine);
                _activeShakeCoroutine = null;
            }

            // 1. Lower EVERY camera in the scene to 0
            var allSceneCams = FindObjectsByType<CinemachineVirtualCameraBase>(FindObjectsSortMode.None);
            foreach (var cam in allSceneCams)
            {
                cam.Priority = EVENT_CAM_PRIORITY_LOW;
                var perlin = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
                if (perlin != null) perlin.AmplitudeGain = 0f;
            }

            // 2. Restore all modified cameras to their specific original states
            foreach (var entry in _modifiedCameras)
            {
                if (entry.Key == null) continue;
                entry.Key.Priority = entry.Value.priority;
                if (entry.Key is CinemachineCamera vcam)
                    vcam.Lens.FieldOfView = entry.Value.fov;
            }

            // 3. FORCE the Golden Camera to be live
            if (_goldenCameraState.IsValid && _goldenCameraState.Camera != null)
            {
                _goldenCameraState.Camera.Priority = Mathf.Max(_goldenCameraState.Priority, DEFAULT_GAMEPLAY_CAM_PRIORITY);
                if (_goldenCameraState.Camera is CinemachineCamera gvcam)
                    gvcam.Lens.FieldOfView = _goldenCameraState.FieldOfView;
            }

            // 4. Reset state for the next event sequence
            _modifiedCameras.Clear();
            _goldenCameraState = default;
        }
    }
}
