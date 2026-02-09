using UnityEditor.Animations;
using UnityEngine;
using MxM;

namespace I_Walk
{
    public class PlayerPakourAnim : MonoBehaviour
    {
        [SerializeField] Animator _playerAnim;
        [SerializeField] RuntimeAnimatorController _playerAnimController;
        [SerializeField] float _castDistance = 3f;
        [SerializeField] LayerMask _castLayer;

        MxMAnimator _mxmAnimator;

        bool _isPakourPlay = false;
        void Start()
        {
            _mxmAnimator = GetComponent<MxMAnimator>();
            _playerAnim.runtimeAnimatorController = null;
            _mxmAnimator.enabled = true;
        }

        void Update()
        {
            if (_isPakourPlay)
            {
                OnParkourEnd();
            }
            else
            {
                PlayerPakourAnimCheck();
            }
        }

        /*
         * Shooting Cast To Forward, if cast is true => running pakour anim
         *                                      false => change 'animator == null'
         */
        void PlayerPakourAnimCheck()
        {
            RaycastHit hit;

            Vector3 rayOrigin = transform.position;
            Vector3 rayDirection = transform.forward;

            if (Physics.Raycast(rayOrigin, rayDirection, out hit ,_castDistance, _castLayer))
            {
                if (_playerAnimController != null) return;

                _playerAnim.runtimeAnimatorController = _playerAnimController;
                _mxmAnimator.enabled = false;

                _isPakourPlay = true;

                // PlayerController에 있는 파쿠르 애니메이션 실행
                // _playerAnim.ParameterSet("", ~~~);
            }
        }

        public void OnParkourEnd()
        {
            AnimatorStateInfo stateInfo = _playerAnim.GetCurrentAnimatorStateInfo(0);
            bool isEnded = stateInfo.normalizedTime >= 1.0;
            //애니메이션의 길이를 재서 그 크기랑 같다면? 코드 실행
            if (isEnded)
            {
                _playerAnim.runtimeAnimatorController = null;
                _mxmAnimator.enabled = true;
                _isPakourPlay = false;
            }
        }

        private void OnDrawGizmos()
        {
            RaycastHit hit;

            Gizmos.color = Color.blue;
            Vector3 origin = transform.position;
            Vector3 direction = transform.forward;

            bool isHit = Physics.Raycast(origin, origin + direction * _castDistance, out hit, _castDistance, _castLayer);

            if (isHit)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(origin, origin + direction * _castDistance);
            }
        }
    }
}
