using UnityEngine;

namespace I_Walk
{
    public class ESCController : MonoBehaviour
    {
        [SerializeField] GameObject _uiPanel;
        [SerializeField] GameObject _optionPanel;


        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // 옵션 패널이 지금 켜져 있는지 꺼져 있는지 상태를 확인합니다.
                bool isOptionOpen = _optionPanel.activeSelf;

                if (isOptionOpen)
                {
                    // 1. 옵션 창이 켜져 있었다면 -> 닫기
                    _optionPanel.SetActive(false);

                    Time.timeScale = 1.0f;
                }
                else
                {
                    // 2. 옵션 창이 꺼져 있었다면 -> 열기
                    _optionPanel.SetActive(true);

                    // 메인 메뉴 UI가 켜져 있다면 같이 겹쳐서 보이지 않게 꺼줍니다.
                    if (_uiPanel.activeSelf)
                    {
                        _uiPanel.SetActive(false);
                    }
                    Time.timeScale = 0f;
                }
            }
        }
    }
}
