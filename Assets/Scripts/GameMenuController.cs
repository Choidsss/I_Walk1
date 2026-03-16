using UnityEngine;
using UnityEngine.UI;

namespace I_Walk
{
    public class GameMenuController : MonoBehaviour
    {
        [SerializeField] Button _startButton;
        [SerializeField] Button _endButton;
        [SerializeField] GameObject _uiPanel;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _uiPanel.SetActive(false);
            _startButton.enabled = false;
            _endButton.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
