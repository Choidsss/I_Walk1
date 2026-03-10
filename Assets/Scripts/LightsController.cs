using UnityEngine;
using TMPro;

namespace I_Walk
{
    public class LightsController : MonoBehaviour
    {
        [SerializeField] GameObject _npc;

        Light _light;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _light = GetComponent<Light>();
            
            TurnOnTheLight();
        }

        // Update is called once per frame
        void Update()
        {
            TurnOffTheLight();
        }

        private void TurnOnTheLight()
        {
            _light.enabled = true;
        }
        
        private void TurnOffTheLight()
        {
            TextMeshProUGUI text = _npc.GetComponentInChildren<TextMeshProUGUI>();

            if(text.text == "¹¹¾ß?")
            {
                _light.enabled = false;
            }
        }
    }
}
