using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UGESystem
{
    /// <summary>
    /// Temporary character controller script providing basic third-person movement based on camera direction for testing purposes.
    /// This version uses InputActionReferences to avoid conflicts with project-specific input settings.
    /// <br/>
    /// 테스트 목적으로 카메라 방향에 기반한 기본적인 3인칭 이동을 제공하는 임시 캐릭터 컨트롤러 스크립트입니다.
    /// 이 버전은 프로젝트 고유의 입력 설정과의 충돌을 방지하기 위해 InputActionReference를 사용합니다.
    /// </summary>
    public class PlayerMove : MonoBehaviour
    {
        [Header("Movement Settings")]
        public Transform cam;
        public float _speed;
        public float _turnSmoothTime = 0.3f;

        [Header("Input References")]
        [SerializeField] private InputActionReference _moveAction;

        const float MAXSPEED = 10.0f;
        float _turnSmoothVelocity;
        float _velocityY = 0f;

        private Vector2 _moveInput;
        Rigidbody _rigidbody;
        Vector3 _move;
        Vector3 _lookDirection = new(0, 0, 0);

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            
            if (_moveAction != null)
            {
                _moveAction.action.Enable();
                _moveAction.action.performed += OnMovePerformed;
                _moveAction.action.canceled += OnMoveCanceled;
            }
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            // Block input if interacting
            if (UGESystemController.Instance.IsInteracting)
            {
                _moveInput = Vector2.zero;
                return;
            }
            _moveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;
        }

        private void OnDisable()
        {
            if (_moveAction != null)
            {
                _moveAction.action.performed -= OnMovePerformed;
                _moveAction.action.canceled -= OnMoveCanceled;
            }
        }

        void FixedUpdate()
        {
            // Reset input if interaction starts mid-move
            if (UGESystemController.Instance.IsInteracting)
            {
                _moveInput = Vector2.zero;
                _speed = 0.0f;
            }

            _move = new(_moveInput.x, _velocityY, _moveInput.y);
            _lookDirection = _move.normalized;

            if (_lookDirection.magnitude >= 0.1f)
            {
                _speed = Mathf.Clamp(_move.magnitude * MAXSPEED, 0.0f, MAXSPEED);
                Move();
            }
            else
            {
                _speed = 0.0f;
            }
        }

        private void Move()
        {
            float targetAngle = Mathf.Atan2(_lookDirection.x, _lookDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _rigidbody.MovePosition(_rigidbody.position + moveDir.normalized * Time.deltaTime * _lookDirection.magnitude * _speed);
        }
    }
}
