using UnityEngine;

namespace I_Walk
{
    public class PlayerMove : MonoBehaviour
    {
        Animator _anim;
        Rigidbody _rigid;

        [SerializeField] GameObject _mainCam;
        [SerializeField] float _rotateSpeed = 10f;
        [SerializeField] float _turnSensitivity = 5f;

        float _moveSpeed;
        float _maxSpeed;
        float _verticalVelocity;
        float _horizontal;
        float _vertical;
        float _turnAmount;
        bool _LShift = false;

        Vector3 _move;
        Vector3 _lookDirection = Vector3.zero;

        void Start()
        {
            _anim = GetComponentInChildren<Animator>();
            _rigid = GetComponent<Rigidbody>();

            // 리지드바디 설정 최적화
            _rigid.interpolation = RigidbodyInterpolation.Interpolate; // 떨림 방지 핵심
            _rigid.constraints = RigidbodyConstraints.FreezeRotation; // 물리 회전 잠금

            if (_mainCam == null) _mainCam = Camera.main.gameObject;
        }

        void Update()
        {
            // 입력 및 방향 설정
            SetDirection();

            // 상태 체크 및 트리거 발동
            AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
            bool isMovingState = stateInfo.IsName("Locomotion") || stateInfo.IsTag("Move"); // BT 노드 이름에 맞춰 수정

            // 멈춤 트리거 로직
            if (_moveSpeed < 0.1f && !_anim.IsInTransition(0))
            {
                if (_maxSpeed == 0.5f) _anim.SetTrigger("stopWalk");
                else if (_maxSpeed == 1f) _anim.SetTrigger("stopRun");

                _maxSpeed = 0f; 
            }

            // startRun,Walk 로직
            
            //if ()
            //{

            //}
        }

        private void FixedUpdate()
        {
            CharacterMove();
            CharacterRotation();
        }

        void SetDirection()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Mouse1))
            {
                _LShift = !_LShift;
            }

            _horizontal = Input.GetAxisRaw("Horizontal");
            _vertical = Input.GetAxisRaw("Vertical");

            Vector3 inputDir = new Vector3(_horizontal, 0, _vertical).normalized;

            if (_mainCam != null && inputDir.magnitude > 0.1f)
            {
                Transform camTrans = _mainCam.transform;

                // [수정 1] camForward.y = 0 대신 ProjectOnPlane 사용 (더 안정적인 벡터 계산)
                Vector3 camForward = Vector3.ProjectOnPlane(camTrans.forward, Vector3.up).normalized;
                Vector3 camRight = Vector3.ProjectOnPlane(camTrans.right, Vector3.up).normalized;

                _move = (camForward * _vertical + camRight * _horizontal).normalized;
                _lookDirection = _move; // 캐릭터가 바라볼 방향만 결정 (계산만 수행)
            }
            else
            {
                _move = Vector3.zero;
            }

            // [수정 2] ★★★ 중요 ★★★ 
            // 기존에 있던 transform.rotation 수정 코드와 _rigid.MoveRotation을 통째로 삭제했습니다.
            // 이유: Update(SetDirection)와 FixedUpdate(CharacterRotation) 양쪽에서 
            // 회전값을 덮어씌우면 서로 실행 주기가 달라 캐릭터가 좌우로 바들바들 떨리게 됩니다.
        }

        void CharacterRotation()
        {
            // [기울기(Banking) 로직 - FixedUpdate에서 실행되므로 고정 주기로 부드럽게 계산됨]
            if (_lookDirection.sqrMagnitude <= 0.01f)
            {
                _turnAmount = Mathf.MoveTowards(_turnAmount, 0f, Time.fixedDeltaTime * _turnSensitivity);
            }
            else if (_vertical > 0.1f && Mathf.Abs(_horizontal) > 0.1f)
            {
                _turnAmount += Mathf.Sign(_horizontal) * Time.fixedDeltaTime * _turnSensitivity;
            }
            else
            {
                _turnAmount = Mathf.MoveTowards(_turnAmount, 0f, Time.fixedDeltaTime * _turnSensitivity);
            }

            _turnAmount = Mathf.Clamp(_turnAmount, -1f, 1f);
            _anim.SetFloat("turnAmount", _turnAmount);

            // [수정 3] 실제 몸 회전 처리를 여기서만 수행
            if (_lookDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_lookDirection);

                // [수정 4] Time.deltaTime 대신 Time.fixedDeltaTime 사용 (물리 주기에 맞춤)
                float lerpRotate = 1f - Mathf.Exp(-_rotateSpeed * Time.fixedDeltaTime);

                // [수정 5] transform.rotation 직접 대입 대신 MoveRotation 사용
                // MoveRotation은 Rigidbody의 Interpolate(보간) 기능과 찰떡궁합이라 떨림을 최종적으로 잡아줍니다.
                _rigid.MoveRotation(Quaternion.Slerp(_rigid.rotation, targetRotation, lerpRotate));
            }
        }

        void CharacterMove()
        {
            float inputMagnitude = new Vector2(_horizontal, _vertical).magnitude;

            if (inputMagnitude > 0.1f)
            {
                _maxSpeed = _LShift ? 1f : 0.5f;
                _moveSpeed = Mathf.MoveTowards(_moveSpeed, _maxSpeed, Time.fixedDeltaTime * 2f);
            }
            else
            {
                _moveSpeed = Mathf.MoveTowards(_moveSpeed, 0f, Time.fixedDeltaTime * 2f);
            }

            _anim.SetFloat("Speed", _moveSpeed, 0.05f, Time.fixedDeltaTime);
        }

        private void OnAnimatorMove()
        {
            if (_anim == null) return;

            Vector3 deltaMove = _anim.deltaPosition;

            // 단순 바닥 체크 (높이가 0.1 이하일 때 바닥으로 간주)
            // 실제로는 레이캐스트나 CharacterController.isGrounded를 쓰는게 좋지만 일단 로직 유지
            bool isGrounded = transform.position.y <= 0.1f;

            if (isGrounded && _verticalVelocity < 0)
            {
                _verticalVelocity = -0.5f; // 바닥에 붙어있도록 살짝 누름
            }
            else
            {
                _verticalVelocity += Physics.gravity.y * Time.fixedDeltaTime;
            }

            deltaMove.y = _verticalVelocity * Time.fixedDeltaTime;

            // MovePosition으로 최종 이동
            _rigid.MovePosition(_rigid.position + deltaMove);
        }
    }
}
