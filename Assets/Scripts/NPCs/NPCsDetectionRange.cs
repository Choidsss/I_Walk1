using UnityEngine;

namespace I_Walk
{
    public class NPCsDetectionRange : MonoBehaviour
    {
        [SerializeField] float _detectionRadius = 0;

        public bool IsPlayer { get; private set; }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                IsPlayer = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                IsPlayer = false;
            }
        }
    }
}
