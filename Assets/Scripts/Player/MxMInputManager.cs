using UnityEngine;
using MxM;

namespace I_Walk
{
    public class MxMInputManager : MonoBehaviour
    {
        MxMTrajectoryGenerator _mxm_T_Generator;
        MxMAnimator _mxmAnimator;

        [SerializeField] float _runSpeed = 6.0f;
        [SerializeField] float _walkSpeed = 1.0f;

        float _targetSpeed;
        
        void Start()
        {
            _mxm_T_Generator = GetComponent<MxMTrajectoryGenerator>();
            _mxmAnimator = GetComponent<MxMAnimator>();

            if (_mxm_T_Generator == null)
                Debug.LogError("Can't find a TrajectoryGenerator!, Check the Component");
            if (_mxmAnimator == null)
                Debug.LogError("Can't find a MxMAnimator!, Check the Component");
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _mxmAnimator.SetRequiredTag("Run");
                _targetSpeed = _runSpeed;
            }
            else
            {
                _mxmAnimator.ClearRequiredTags();
                _targetSpeed = _walkSpeed;
            }

            _mxm_T_Generator.MaxSpeed = Mathf.Lerp(_mxm_T_Generator.MaxSpeed, _targetSpeed, Time.deltaTime * 5f);
        }
    }
}
