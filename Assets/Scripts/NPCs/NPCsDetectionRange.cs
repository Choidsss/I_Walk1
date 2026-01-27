using UnityEngine;

namespace I_Walk
{
    public class NPCsDetectionRange : MonoBehaviour
    {
        [SerializeField] float _detectionRadius = 0;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        }
    }
}
