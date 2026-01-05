using UnityEngine;

namespace I_Walk
{
    public class PlayerRootMove : MonoBehaviour
    {
        Rigidbody _rb;
        Animator _anim;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _rb = GetComponentInParent<Rigidbody>();
            _anim = GetComponent<Animator>();
        }

        private void OnAnimatorMove()
        {
            if (_anim.applyRootMotion)
            {
                _rb.MovePosition(_rb.position + _anim.deltaPosition);
                _rb.MoveRotation(_anim.rootRotation);
                transform.localPosition = Vector3.zero;
            }
        }
    }
}
