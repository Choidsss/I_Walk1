using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

namespace I_Walk
{
    public class GameMenuController : MonoBehaviour
    {
        [SerializeField] GameObject _uiPanel;
        [SerializeField] GameObject _optionPanel;
        [SerializeField] GameObject _pausePanel;
        [SerializeField] Button _startButton;
        [SerializeField] Button _endButton;
        [SerializeField] Button _optionButton;
        [SerializeField] Button _backButton;
        [SerializeField] Button _continueButton;
        [SerializeField] Button _mainButton;
        [SerializeField] CinemachineCamera _startCam;
        [SerializeField] CinemachineCamera _freeLookCam;

        [Header("Slider")]
        [SerializeField] Slider _slider;

        [Header("Sounds")]
        [SerializeField] GameSounds _audioManager;
        [SerializeField] VolumeController _volume;

        private void Awake()
        {
            _startCam.Priority = 100;
            _freeLookCam.Priority = 30;

            _startCam.enabled = true;
            _freeLookCam.enabled = false;

            _audioManager.GameMenuUISoundStart();
        }

        void Start()
        {
            //버튼이 눌리기 전까지는 게임 정지
            Time.timeScale = 0;

            _uiPanel.SetActive(true);
            _optionPanel.SetActive(false);
            _pausePanel.SetActive(false);

            _startButton.onClick.AddListener(ClickedStartButton);
            _endButton.onClick.AddListener(ClickedEndButton);
            _optionButton.onClick.AddListener(ClickedOptionButton);
            _backButton.onClick.AddListener(ClickedReturnMenuButton);
            _slider.onValueChanged.AddListener(ChangeVolume);
            _continueButton.onClick.AddListener(OnClickedContinue);
            _mainButton.onClick.AddListener(OnClickedMain);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GetButtonDownEscape();
            }
        }

        private void GameReset()
        {
            // 주의: 씬을 다시 부르기 전에 멈춰있던 시간을 꼭 1배속으로 돌려놔야 합니다!
            // 안 그러면 씬이 재시작되자마자 시간이 멈춰있는 버그가 생깁니다.
            Time.timeScale = 1.0f;

            // 현재 켜져있는 씬(Scene)의 번호를 가져와서 아예 새로고침(재로딩) 해버립니다.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void GetButtonDownEscape()
        {
            //  1. 철통 방어: 메인 메뉴이거나 옵션 창이 켜져있다면 ESC 무조건 무시! 
            // (오직 UI에 있는 '뒤로가기' 버튼으로만 나갈 수 있게 강제합니다)
            if (_uiPanel.activeSelf || _optionPanel.activeSelf) return;

            //  2. 퍼즈 창만 켜져있다면 -> 퍼즈 창 닫고 게임 재개!
            if (_pausePanel.activeSelf)
            {
                _pausePanel.SetActive(false);
                Time.timeScale = 1.0f;

                // 게임으로 돌아가니까 마우스 다시 숨기기
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            //  3. 다 꺼져있고 평화롭게 게임 중이었다면 -> 퍼즈 창 켜기!
            else
            {
                _pausePanel.SetActive(true);
                Time.timeScale = 0.0f;

                // 퍼즈 메뉴 열렸으니까 마우스 보이게 풀어주기
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void OnClickedContinue()
        {
            Time.timeScale = 1.0f;

            _uiPanel.SetActive(false);
            _optionPanel.SetActive(false);
            _pausePanel.SetActive(false);
        }

        private void OnClickedMain()
        {
            // 메인으로 돌아가기 버튼을 누르면 게임을 싹 다 리셋해버립니다.
            GameReset();
        }

        private void ClickedStartButton()
        {
            _startCam.Priority = 0;

            _startCam.enabled = false;
            _freeLookCam.enabled = true;

            _uiPanel.SetActive(false);
            Time.timeScale = 1.0f;

            _audioManager.GameMenuUISoundStop();
            _audioManager.ClickSound();
        }

        private void ClickedOptionButton()
        {
            _uiPanel.SetActive(false);
            _optionPanel.SetActive(true);
        }

        private void ClickedReturnMenuButton()
        {
            _optionPanel.SetActive(false);
            _uiPanel.SetActive(true);
        }

        private void ChangeVolume(float value)
        {
            _volume.SetMasterVolume(value);
        }

        private void ClickedEndButton()
        {
            _uiPanel.SetActive(false);
            _audioManager.GameMenuUISoundStop();
            _audioManager.ClickSound();

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
