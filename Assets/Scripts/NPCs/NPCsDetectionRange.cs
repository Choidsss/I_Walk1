using UnityEngine;

namespace I_Walk
{
    public class NPCsDetectionRange : MonoBehaviour
    {
        [SerializeField] float _detectionRadius = 0;

        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere()
        }
    }
}
