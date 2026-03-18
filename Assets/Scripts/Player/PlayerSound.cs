using UnityEngine;

namespace I_Walk
{
    public class PlayerSound : MonoBehaviour
    {
        [Header("AudioClips")]
        [SerializeField] AudioClip _walkSound;
        [SerializeField] AudioClip _failSound;

        [Header("AudioSource")]
        [SerializeField] AudioSource _audioSource;

        [Header("Footstep Settings")]
        [SerializeField] float _stepDistance = 1.2f; // 소리가 날 이동 거리 간격 (인스펙터에서 조절 가능)

        // 거리 계산을 위해 위치를 기억해둘 변수들
        private Vector3 _lastPosition;
        private float _distanceMoved = 0f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // 게임 시작 시점의 위치를 초기값으로 저장
            _lastPosition = transform.position;
        }

        private void Update()
        {
            // 1. 이전 프레임 위치와 현재 위치의 거리 차이(이동량) 계산
            float moveDelta = Vector3.Distance(transform.position, _lastPosition);

            // 2. 이동한 거리를 계속 누적
            _distanceMoved += moveDelta;

            // 3. 누적된 거리가 설정한 보폭(_stepDistance)을 넘으면?
            if (_distanceMoved >= _stepDistance)
            {
                // 발소리 재생 함수 호출!
                WalkingSound();

                // 거리를 초기화해서 다시 0부터 재도록 만듦
                _distanceMoved = 0f;
            }

            // 4. 다음 프레임 계산을 위해 방금 위치를 과거 위치로 덮어쓰기
            _lastPosition = transform.position;
        }

        public void WalkingSound()
        {
            if (_walkSound != null)
            {
                _audioSource.PlayOneShot(_walkSound);
            }
        }

        public void FailSound()
        {
            if (_failSound != null)
            {
                _audioSource.PlayOneShot(_failSound);
            }
        }
    }
}
