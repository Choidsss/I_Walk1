using UnityEngine;
using MxM;

namespace I_Walk
{
    public class PlayerFastMove : MonoBehaviour
    {
        MxMAnimator _mxmAnimator;

        public float Speed { get; set; } = 1.5f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _mxmAnimator = GetComponent<MxMAnimator>();
        }

        // Update is called once per frame
        void Update()
        {
            AnimationGetTwice();
        }

        void AnimationGetTwice()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _mxmAnimator.PlaybackSpeed = Speed;
            }
            else
            {
                _mxmAnimator.PlaybackSpeed = 1.0f;
            }
                
        } 
    }
}
