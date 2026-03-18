using UnityEngine;
using Unity.Cinemachine;

namespace I_Walk
{
    public class DollyCartTrigger : MonoBehaviour
    {
        [SerializeField] GameObject _dollyCamera; 
        [SerializeField] float _moveSpeed = 1.0f;

        
        public void StartEndingDollyCartSpline()
        {
            if (_dollyCamera != null)
            {
                CinemachineSplineDolly CinemachineSplineDolly = _dollyCamera.GetComponent<CinemachineSplineDolly>();

                //CinemachineSplineDolly.AutomaticDolly.
            }
        } 
    }
}
