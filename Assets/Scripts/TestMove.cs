using UnityEngine;

namespace I_Walk
{
    public class TestMove : MonoBehaviour
    {
        Animator _anim;
        Rigidbody _rigid;
        CharacterController _controller;

        [SerializeField] GameObject _mainCam;
        [SerializeField] float _moveSpeed = 5f;     
        [SerializeField] float _rotateSpeed = 10f;

        float _verticalVelocity;
        float _horizontal;
        float _vertical;
        bool _LShift = false;

        Vector3 _move;
        Vector3 _lookDirection = new Vector3(0,0,0);

        void Start()
        {
            _anim = GetComponentInChildren<Animator>();
            _rigid = GetComponent<Rigidbody>();
            _controller = GetComponent<CharacterController>();

            if (_mainCam == null)
            {
                _mainCam = Camera.main.gameObject;
            }
        }

        void Update()
        {
            SetDirection();           
        }

        private void FixedUpdate()
        {
            CharacterMove();
        }

        void SetDirection()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Mouse1))
            {
                _LShift = !_LShift; // ����Ʈ ���
            }

            _horizontal = Input.GetAxisRaw("Horizontal");
            _vertical = Input.GetAxisRaw("Vertical");

            //����ī�޶� ���� ���� ����ֱ�
            if (_mainCam != null)
            {
                Transform camTrans = _mainCam.transform;
                Vector3 camForward = camTrans.forward;
                Vector3 camRight = camTrans.right;

                camForward.y = 0;
                camRight.y = 0;

                _move = (camForward.normalized * _vertical + camRight.normalized * _horizontal).normalized;
            }
            else
            {
                _move = new Vector3(_horizontal, 0, _vertical).normalized;
            }

            _lookDirection = _move;

            if (_lookDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_lookDirection);

                float lerpPct = 1f - Mathf.Exp(-_rotateSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpPct);
                // _rigid.MoveRotation(targetRotation);
            }
        }

        void CharacterRotation()
        {

        }

        void CharacterMove()
        {
            float inputMagnitude = new Vector2(_horizontal, _vertical).magnitude;
            float finalAnimSpeed = 0f;

            if (inputMagnitude > 0.1f)
            {
                finalAnimSpeed = _LShift ? 5f : 2.5f;
            }

            _anim.SetFloat("Speed", finalAnimSpeed * inputMagnitude, 0.1f, Time.deltaTime);

            //if (inputMagnitude > 0.1f)
            //{
            //    // 1. 목표 속도 결정 ** 수정필요** 내가 설정한 _moveSpeed에서 값이 설정되도록
            //    float targetSpeed = _LShift ? 5f : 2.5f;
            //    finalAnimSpeed = inputMagnitude * targetSpeed;

            //    Vector3 moveVel = _move * finalAnimSpeed;

            //    moveVel.y = _rigid.linearVelocity.y;

            //    _rigid.linearVelocity = moveVel;
            //}
            //else
            //{
            //    _rigid.linearVelocity = new Vector3(0, _rigid.linearVelocity.y, 0);
            //}
        }

        private void OnAnimatorMove()
        {
            if (!_anim.applyRootMotion)
            {
                _anim.applyRootMotion = true;
            }
            
            if (_anim == null) return;
            
            Vector3 deltaMove = _anim.deltaPosition;

            if (_controller.isGrounded)
            {
                _verticalVelocity = -0.5f;
            }
            else
            {
                _verticalVelocity += Physics.gravity.y * Time.fixedDeltaTime;
            }

            deltaMove.y = _verticalVelocity * Time.fixedDeltaTime;

            _controller.Move(_rigid.position + deltaMove);
        }
    }
}

    
