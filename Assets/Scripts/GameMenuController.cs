using Unity.VisualScripting;
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
            _uiPanel.SetActive(true);

            //버튼이 눌리기 전까지는 게임 정지
            Time.timeScale = 0;

            _startButton.onClick.AddListener(ClickedStartButton);
            _endButton.onClick.AddListener(ClickedEndButton);
        }

        private void ClickedStartButton()
        {
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
