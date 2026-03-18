using UnityEngine;
using UnityEngine.Audio;

namespace I_Walk
{
    public class VolumeController : MonoBehaviour
    {
        [Header("오디오 믹서 연결")]
        [SerializeField] private AudioMixer _audioMixer;

        // 볼륨 조절 함수 (슬라이더의 OnValueChanged 이벤트에 연결할 거야!)
        // 인자로 float sliderValue (0.0001f ~ 1f)를 받아옴
        public void SetMasterVolume(float sliderValue)
        {
            // 믹서의 파라미터 이름("MasterVolume")과 변환된 데시벨(dB) 값을 넣어줌
            _audioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
        }
    }
}
