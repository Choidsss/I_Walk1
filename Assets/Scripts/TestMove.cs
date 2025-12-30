using UnityEngine;

namespace I_Walk
{
    public class TestMove : MonoBehaviour
    {
        Animator _anim;

        [SerializeField] float _moveSpeed = 5f;     // 실제 이동 속도
        [SerializeField] float _rotateSpeed = 10f; // 회전 속도

        float _horizontal;
        float _vertical;

        void Start()
        {
            _anim = GetComponent<Animator>();
        }

        void Update()
        {
            // 1. 입력 받기 (WASD 또는 화살표)
            _horizontal = Input.GetAxis("Horizontal");
            _vertical = Input.GetAxis("Vertical");

            // 2. 이동 방향 계산
            Vector3 moveDir = new Vector3(_horizontal, 0, _vertical).normalized;

            // 3. 애니메이션 Speed 파라미터 전달
            // .magnitude를 쓰면 대각선 이동 시에도 적절한 속도값(0~1 사이)이 전달됩니다.
            float moveMagnitude = moveDir.magnitude;
            _anim.SetFloat("Speed", moveMagnitude);

            // 4. 실제로 이동 및 회전 처리
            if (moveMagnitude > 0.1f)
            {
                // 이동
                transform.position += moveDir * _moveSpeed * Time.deltaTime;

                // 캐릭터가 이동 방향을 부드럽게 바라보게 함
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
            }
        }
    }
}
