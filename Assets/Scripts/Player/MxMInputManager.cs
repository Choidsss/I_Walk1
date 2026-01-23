using UnityEngine;
using MxM;

namespace I_Walk
{
    public class MxMInputManager : MonoBehaviour
    {
        MxMTrajectoryGenerator _mxm_T_Generator;
        //MxMAnimator _mxmAnimator; //BodyVelocity 체크용 => 추후 삭제 

        [SerializeField] float _targetMaxSpeed = 0.7f;
        [SerializeField] float _runSpeed = 6.0f;
        [SerializeField] float _walkSpeed = 1.0f;
        //[SerializeField] float _jogSpeed = 1.7f;

        void Start()
        {
            _mxm_T_Generator = GetComponent<MxMTrajectoryGenerator>();

            if (_mxm_T_Generator == null)
                Debug.LogError("TrajectoryGenerator 못 찾음!");
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _mxm_T_Generator.MaxSpeed = _runSpeed; // 달리기
            }
            //else if (Input.GetKey(KeyCode.LeftControl))
            //{
            //    _mxm_T_Generator.MaxSpeed = _jogSpeed; //조깅
            //}
            else
            {
                _mxm_T_Generator.MaxSpeed = _walkSpeed; // 걷기
            }

            _mxm_T_Generator.MaxSpeed = Mathf.Lerp(_mxm_T_Generator.MaxSpeed, _targetMaxSpeed, Time.deltaTime * 5f);

            //Debug.Log($"MaxSpeed: {_mxm_T_Generator.MaxSpeed:F2}");
            //Debug.Log($"BodyVelocity: {_mxmAnimator.BodyVelocity.magnitude:F2}");
        }

        
    }
}
