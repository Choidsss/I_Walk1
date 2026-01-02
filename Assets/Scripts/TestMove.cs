using UnityEngine;

namespace I_Walk
{
    public class TestMove : MonoBehaviour
    {
        Animator _anim;
        Rigidbody _rigid;

        [SerializeField] GameObject _mainCam;
        [SerializeField] float _moveSpeed = 5f;     // 실제 이동 속도
        [SerializeField] float _rotateSpeed = 10f; // 회전 속도

        float _horizontal;
        float _vertical;
        bool _LShift = false;

        Vector3 _move;
        Vector3 _lookDirection = new Vector3(0,0,0);

        void Start()
        {
            _anim = GetComponent<Animator>();
            _rigid = GetComponent<Rigidbody>();

            if (_mainCam == null)
            {
                _mainCam = Camera.main.gameObject;
            }
        }

        void Update()
        {
            SetDirection();
            CharacterMove();
        }

        void SetDirection()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Mouse1))
            {
                _LShift = !_LShift; // 쉬프트 토글
            }

            _horizontal = Input.GetAxisRaw("Horizontal");
            _vertical = Input.GetAxisRaw("Vertical");

            //메인카메라 기준 방향 잡아주기
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
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
            }
        }

        void CharacterMove()
        {
            float inputMagnitude = new Vector2(_horizontal, _vertical).magnitude;
            float finalAnimSpeed = 0f;

            //쉬프트가 눌린걸 판단해서 속도의 임계치 제어 => 안눌리면 BT에서 걷는모션이 나오고, 눌리면 뛰는모션
            if (inputMagnitude > 0.1f)
            {
                float currentSpeedLimit = _LShift ? 5f : 2.5f;
                finalAnimSpeed = Mathf.Clamp(inputMagnitude * _moveSpeed, 0, currentSpeedLimit);
            }

            _anim.SetFloat("Speed", finalAnimSpeed, 0.1f,  Time.deltaTime);
        }

        private void OnAnimatorMove()
        {

        }
    }
}
