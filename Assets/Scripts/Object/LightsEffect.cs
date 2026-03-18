using UnityEngine;

namespace I_Walk
{
    public class LightsEffect : MonoBehaviour
    {
        Light _light;

        void Start()
        {
            _light = GetComponent<Light>();
        }

        void Update()
        {
            TurnOnLight();
        }

        public void TurnOnLight()
        {
            _light.enabled = true;
        }
    }
}
