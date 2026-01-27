using UnityEngine;

namespace I_Walk
{
    public class NPCsEventMarkUI : MonoBehaviour
    {
        [SerializeField] GameObject _bangMark;

        bool _isPlayer = false;


        //****************ToDo : Player Checking************************
        //방법 1 : 콜리더를 크게 하나 트리거로 만들어서 안에 들어왔는지 체크
        //방법 2 : SerializeField로 Player를 집어넣어서 얘가 있는지 없는지 체크
        //방법 3 : 안에 있는 애들을 배열에 넣고 빼서, 태그가 Player인 애 찾는방법
        void Start()
        {
            _bangMark.SetActive(false);   
        }

        // Update is called once per frame
        void Update()
        {
            ShowBangMark();
        }


        void ShowBangMark()
        {
            if(_isPlayer == true)
            {
                _bangMark.SetActive(true);
            }
            else
            {
                _bangMark.SetActive(false);
            }
        }
    }
}
