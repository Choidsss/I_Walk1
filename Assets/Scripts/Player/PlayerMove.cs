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
        bool _startRun = false;
        protected bool _LShift = false;

        public bool LeftShiftPush { get { return _LShift; } protected set { _LShift = value; } }

        Vector3 _move;
        Vector3 _lookDirection = Vector3.zero;

        void Start()
        {
            _anim = GetComponentInChildren<Animator>();
            _rigid = GetComponent<Rigidbody>();

            _rigid.interpolation = RigidbodyInterpolation.Interpolate; 
            _rigid.constraints = RigidbodyConstraints.FreezeRotation; 

            if (_mainCam == null) _mainCam = Camera.main.gameObject;
        }

        void Update()
        {
            SetDirection();
            StopAnimation();
            StartRunAnim();
            
        }

        private void FixedUpdate()
        {
            CharacterMove();
            CharacterRotation();
        }

        void SetDirection()
        {
            _horizontal = Input.GetAxisRaw("Horizontal");
            _vertical = Input.GetAxisRaw("Vertical");

            Vector3 inputDir = new Vector3(_horizontal, 0, _vertical).normalized;

            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Mouse1))
            {
                _LShift = true;
                _startRun = true;
            }
            else
            {
                _startRun = false;
            }


            if (_mainCam != null && inputDir.magnitude > 0.1f)
            {
                Transform camTrans = _mainCam.transform;

                Vector3 camForward = Vector3.ProjectOnPlane(camTrans.forward, Vector3.up).normalized;
                Vector3 camRight = Vector3.ProjectOnPlane(camTrans.right, Vector3.up).normalized;

                _move = (camForward * _vertical + camRight * _horizontal).normalized;
                _lookDirection = _move; 
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

        void StartRunAnim()
        {
            if (_startRun)
            {
                _anim.SetBool("startRun", true);
            }
            else
            {
                _anim.SetBool("startRun", false);
            }
        }

        void StopAnimation()
        {
            if (_moveSpeed < 0.1f)
            {
                if (_maxSpeed == 0.5f)
                {
                    _anim.SetTrigger("stopWalk");
                    _LShift = false;
                }
                else if (_maxSpeed == 1f)
                {
                    _anim.SetTrigger("stopRun");
                    _LShift = false;
                }
            }
        }

        void CharacterRotation()
        {
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

            if (_lookDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_lookDirection);
                float lerpRotate = 1f - Mathf.Exp(-_rotateSpeed * Time.fixedDeltaTime);

                _rigid.MoveRotation(Quaternion.Slerp(_rigid.rotation, targetRotation, lerpRotate));
            }
        }

        void CharacterMove()
        {
            float inputMagnitude = new Vector2(_horizontal, _vertical).magnitude;

            if (inputMagnitude > 0.1f)
            {
                if (_startRun)
                {
                    _maxSpeed = 3.0f;
                    _moveSpeed = 3.0f; 

                    _anim.SetFloat("Speed", 1.0f);
                }
                else
                {
                    _maxSpeed = _LShift ? 1f : 0.5f;
                    _moveSpeed = Mathf.MoveTowards(_moveSpeed, _maxSpeed, Time.fixedDeltaTime * 2f);

                    _anim.SetFloat("Speed", _moveSpeed, 0.05f, Time.fixedDeltaTime);
                }
            }
            else
            {
                _moveSpeed = Mathf.MoveTowards(_moveSpeed, 0f, Time.fixedDeltaTime * 2f);
                _anim.SetFloat("Speed", _moveSpeed, 0.05f, Time.fixedDeltaTime);
            }
        }

        private void OnAnimatorMove()
        {
            if (_anim == null) return;

            Vector3 deltaMove = _anim.deltaPosition;

            bool isGrounded = transform.position.y <= 0.1f;

            if (isGrounded && _verticalVelocity < 0)
            {
                _verticalVelocity = -0.5f;
            }
            else
            {
                _verticalVelocity += Physics.gravity.y * Time.fixedDeltaTime;
            }

            deltaMove.y = _verticalVelocity * Time.fixedDeltaTime;

            _rigid.MovePosition(_rigid.position + deltaMove);
        }
    }
}
