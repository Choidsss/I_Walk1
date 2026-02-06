using UGESystem;
using UnityEngine;
using TMPro;

namespace I_Walk
{
    public class NPCEmotions : MonoBehaviour
    {
        Animator _anim;
        
        [SerializeField] TextMeshProUGUI _tmpGUI;

        /*
         * 글자 찍히는거 => 확인 완료
         * 
         * ToDo : 특정 문자열이 찍히면 그에 맞는 이모션이 나가도록 ==> 테스트 완료
         */
        void Start()
        {
            _anim = GetComponent<Animator>();
        }

        void Update()
        {
            if (_tmpGUI.text == "good job")
            {
                _anim.SetTrigger("Cocky");
            }
        }
    }
}
