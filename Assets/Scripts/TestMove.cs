using UnityEngine;

namespace I_Walk
{
    public class TestMove : MonoBehaviour
    {
        Animator _anim;
        Rigidbody _rigid;

        [SerializeField] float _moveSpeed = 5f;     // 실제 이동 속도
        [SerializeField] float _rotateSpeed = 10f; // 회전 속도

        float _horizontal;
        float _vertical;

        Vector3 _move;
        Vector3 _lookDirection = new Vector3(0,0,0);

        void Start()
        {
            _anim = GetComponent<Animator>();
            _rigid = GetComponent<Rigidbody>();
        }

        void Update()
        {
            SetDirection();
            CharacterMove();
        }

        void SetDirection()
        {
            _horizontal = Input.GetAxis("Horizontal");
            _vertical = Input.GetAxis("Vertical");

            _move = new Vector3(_horizontal, 0, _vertical);
            _lookDirection = _move.normalized;

            if (_lookDirection.magnitude > 0.1f)
            {
                // 그 방향을 바라보도록
                Quaternion targetRotation = Quaternion.LookRotation(_lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
            }
        }

        void CharacterMove()
        {
            // 입력의 크기를 계산 (0 ~ 1 사이)
            float moveMagnitude = _move.magnitude;

            // Animator에 미리 선언된 'Speed' 파라미터에 값을 전달합니다.
            // (파라미터 이름이 다르다면 "Speed" 대신 해당 이름을 넣으세요)
            _anim.SetFloat("Speed", moveMagnitude * _moveSpeed);

            Debug.Log(moveMagnitude * _moveSpeed);
        }
    }
}
