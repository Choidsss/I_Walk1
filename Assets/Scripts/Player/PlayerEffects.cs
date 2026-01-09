using UnityEngine;

namespace I_Walk
{
    public class PlayerEffects : MonoBehaviour
    {
        [SerializeField] GameObject _dashEffect;

        GameObject _currentDashEffect;
        PlayerMove _playerMove;

        void Start()
        {
            _playerMove = GetComponent<PlayerMove>();
        }

        void Update()
        {
            DashEffect();
        }


        void DashEffect()
        {
            if (_dashEffect == null || _playerMove == null) return;

            if (_playerMove.LeftShiftPush)
            {
                if (_currentDashEffect == null)
                {
                    // [수정 핵심] 플레이어의 위치에서 뒤쪽으로 0.5f만큼 이동한 지점을 계산합니다.
                    // transform.forward는 앞쪽이므로, -를 붙여서 뒤쪽 방향을 만듭니다.
                    float backDistance = 5.5f; // 뒤로 보낼 거리 (수치를 조절해 보세요)
                    Vector3 spawnPosition = this.transform.position - (this.transform.forward * backDistance);

                    // 계산된 spawnPosition에 생성하고, 여전히 따라오도록 부모(this.transform)를 지정합니다.
                    _currentDashEffect = Instantiate(_dashEffect, spawnPosition, this.transform.rotation, this.transform);
                }
            }
            else
            {
                if (_currentDashEffect != null)
                {
                    Destroy(_currentDashEffect);
                    _currentDashEffect = null;
                }
            }
        }
    }
}
