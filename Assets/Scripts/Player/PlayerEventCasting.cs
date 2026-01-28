using UnityEngine;

namespace I_Walk
{
    public class PlayerEventCasting : MonoBehaviour
    {
        [Header("Casting Options")]
        [SerializeField] float _castRange = 1.0f;

        [SerializeField] Transform _startPos;

        bool _isHit = false;

        // *********************ToDo : 키 입력 부분은 같은 스크립트로 묶어두기************************
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                PlayerRaycast();
            }
        }

        void PlayerRaycast()
        {
            Ray ray = new Ray(_startPos.position , transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray,out hit, _castRange))
            {
                _isHit = true;
                Debug.Log($"맞은 대상 : {hit.collider.name}");
            }
            else
            {
                _isHit = false;
            }
        }
        
        //private void OnDrawGizmos()
        //{
        //    if (_startPos == null) return;

        //    Gizmos.color = Color.red;

        //    Gizmos.DrawLine(_startPos.position, _startPos.position + transform.forward);
        //    Gizmos.DrawSphere(_startPos.position, 0.05f);
        //}
    }
}
