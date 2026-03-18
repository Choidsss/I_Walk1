using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

namespace I_Walk
{
    public class GameMenuController : MonoBehaviour
    {
        [SerializeField] GameObject _uiPanel;
        [SerializeField] Button _startButton;
        [SerializeField] Button _endButton;
        [SerializeField] CinemachineCamera _startCam;
        [SerializeField] CinemachineCamera _freeLookCam;

        private void Awake()
        {
            _startCam.Priority = 100;
            _freeLookCam.Priority = 30;

            _startCam.enabled = true;
            _freeLookCam.enabled = false;
        }

        void Start()
        {
            //버튼이 눌리기 전까지는 게임 정지
            Time.timeScale = 0;

            _uiPanel.SetActive(true);

            _startButton.onClick.AddListener(ClickedStartButton);
            _endButton.onClick.AddListener(ClickedEndButton);
        }

        private void ClickedStartButton()
        {
            _startCam.Priority = 0;

            _startCam.enabled = false;
            _freeLookCam.enabled = true;

            _uiPanel.SetActive(false);
            Time.timeScale = 1.0f;
        }

        private void ClickedEndButton()
        {
            _uiPanel.SetActive(false);


#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            Debug.Log("Quit Game");
#else
     Application.Quit();

#endif
        }


        private void EndingSceneAfter()
        {
            //엔딩이 나오고 나면 일정시간이 지나고 메뉴창 켜기

        }
    }
}
