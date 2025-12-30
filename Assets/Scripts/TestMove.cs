using UnityEngine;

namespace I_Walk
{
    public class TestMove : MonoBehaviour
    {
        Animator _anim;

        [SerializeField] float _speed = 0;

        float _horizontal;
        float _vertical;

        Vector3 move;
        Vector3 _lookDirection;



        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
