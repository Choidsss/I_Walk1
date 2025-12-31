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

        Vector3 _move;
        Vector3 _lookDirection;

        void Start()
        {
            _anim = GetComponent<Animator>();
        }

        void Update()
        {
            // 1. 입력 받기 (WASD 또는 화살표)
            _horizontal = Input.GetAxis("Horizontal");
            _vertical = Input.GetAxis("Vertical");


        }
    }
}
