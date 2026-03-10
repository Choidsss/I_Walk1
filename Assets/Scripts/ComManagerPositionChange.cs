using UnityEngine;

namespace I_Walk
{
    public class ComManagerPositionChange : MonoBehaviour
    {
        [SerializeField] Transform _nextComManagerTransform;
        
        bool _comManager = true;

        void Update()
        {
            if (_comManager)
            {
                Instantiate(this.gameObject, _nextComManagerTransform);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.name == "Player")
            {
                _comManager = false;
                Destroy(this.gameObject);
            }
        }
    }
}
