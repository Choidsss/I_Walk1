using UnityEngine;

namespace I_Walk
{
    public class PlayerMove : MonoBehaviour
    {
        Animator _anim;
        Rigidbody _rigid;

        [SerializeField] GameObject _mainCam;
        [SerializeField] GameObject _freelookCam;
        [SerializeField] float _moveSpeed = 5f;
        [SerializeField] float _rotateSpeed = 10f;
        [SerializeField] float _turnSensitivity = 5f;

        float _verticalVelocity;
        float _horizontal;
        float _vertical;
        float _turnAmount;
        bool _LShift = false;

        Vector3 _move;
        Vector3 _lookDirection = new Vector3(0, 0, 0);

        void Start()
        {
            _anim = GetComponentInChildren<Animator>();
            _rigid = GetComponent<Rigidbody>();

            if (_mainCam == null)
            {
                _mainCam = Camera.main.gameObject;
            }
        }

        void Update()
        {
            SetDirection();

            /*
             * ************************************
             * 멈추는 애니메이션 나오게 하기(조건: 속도가 0일때, 걷고 있을때 or 뛰고 있을때)
             * ************************************
             */
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
                Vector3 camForward = camTrans.forward;
                Vector3 camRight = camTrans.right;

                camForward.y = 0;
                camRight.y = 0;

                _move = (camForward * _vertical + camRight * _horizontal).normalized;

                _lookDirection = _move;
            }
            else
            {
                _move = Vector3.zero;
            }

            if (_lookDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_lookDirection);

                float lerpPct = 1f - Mathf.Exp(-_rotateSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpPct);
                _rigid.MoveRotation(targetRotation);
            }
        }

        /*
         * 기울어지는 애니메이션 다듬기 => 떨리는 문제 확인
         */
        void CharacterRotation()
        {
            if (_lookDirection.sqrMagnitude <= 0.01f)
            {
                _turnAmount = Mathf.MoveTowards(_turnAmount, 0f, Time.fixedDeltaTime * _turnSensitivity);
                _anim.SetFloat("turnAmount", _turnAmount);
                return;
            }

            if (Mathf.Abs(_horizontal) > 0.1f)
            {
                _turnAmount += Mathf.Sign(_horizontal) * Time.fixedDeltaTime * _turnSensitivity;
            }
            else
            {
                _turnAmount = Mathf.MoveTowards(_turnAmount, 0f, Time.fixedDeltaTime * _turnSensitivity);
            }

            _turnAmount = Mathf.Clamp(_turnAmount, -1f, 1f);
            _anim.SetFloat("turnAmount", _turnAmount);

            Quaternion targetRotation = Quaternion.LookRotation(_lookDirection);
            float lerpRotate = 1f - Mathf.Exp(-_rotateSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpRotate);

            // Quaternion targetRotation = Quaternion.LookRotation(_lookDirection);
            // float targetTurn = Mathf.Clamp(_turnAmount, -1.0f, 1.0f);
            // _turnAmount = Mathf.Lerp(_turnAmount, targetTurn, Time.fixedDeltaTime * _turnSensitivity);
        }

        /*
         * **************************************************
         * 속도를 _moveSpeed가 아닌 지역변수로 받고 있는 문제
         * **************************************************
         */
        void CharacterMove()
        {
            float inputMagnitude = new Vector2(_horizontal, _vertical).magnitude;
            float finalAnimSpeed = 0f;

            if (inputMagnitude > 0.1f)
            {
                finalAnimSpeed = _LShift ? 1f : 0.5f;
            }

            _anim.SetFloat("Speed", finalAnimSpeed * inputMagnitude, 0.1f, Time.deltaTime);
        }

        private void OnAnimatorMove()
        {
            if (!_anim.applyRootMotion)
            {
                _anim.applyRootMotion = true;
            }

            if (_anim == null) return;

            Vector3 deltaMove = _anim.deltaPosition;

            _verticalVelocity += Physics.gravity.y * Time.fixedDeltaTime;

            deltaMove.y = _verticalVelocity * Time.fixedDeltaTime;

            _rigid.MovePosition(_rigid.position + deltaMove);
        }
    }
}
