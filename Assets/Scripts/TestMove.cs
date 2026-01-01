using UnityEngine;

namespace I_Walk
{
    public class TestMove : MonoBehaviour
    {
        Animator _anim;
        Rigidbody _rigid;

        [SerializeField] GameObject _TPSCam;
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
        }

        void Update()
        {
            SetDirection();
            CharacterMove();
        }

        void SetDirection()
        {
            //왼쪽 쉬프트 눌렀는지 안눌렀는지 체크
            if (Input.GetKey(KeyCode.LeftShift)) _LShift = true;
            else _LShift = false;

            _horizontal = Input.GetAxis("Horizontal");
            _vertical = Input.GetAxis("Vertical");

            Transform cameraTransform = _TPSCam.transform;



            _move = new Vector3(_horizontal, 0, _vertical);
            _lookDirection = _move.normalized;

            if (_lookDirection.magnitude > 0.1f)
            {
                // 그 방향을 바라보도록
                Quaternion targetRotation = Quaternion.LookRotation(_lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
            }
        }

        //Left Shift를 눌러야만 뛰는 속도를 넘어가도록
        void CharacterMove()
        {
            float moveMagnitude = _move.magnitude;

            _anim.SetFloat("Speed", moveMagnitude * _moveSpeed);
        }

        //private void OnAnimatorMove()
        //{
            
        //}
    }
}
