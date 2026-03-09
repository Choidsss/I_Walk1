using UnityEngine;

namespace I_Walk
{
    public class LightsController : MonoBehaviour
    {
        Light _light;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _light = GetComponent<Light>();
        }

        // Update is called once per frame
        void Update()
        {
            TurnOnTheLight();
        }

        private void TurnOnTheLight()
        {
            _light.enabled = true;
        }
        
        private void TurnOffTheLight()
        {
            _light.enabled = false;
        }
    }
}
