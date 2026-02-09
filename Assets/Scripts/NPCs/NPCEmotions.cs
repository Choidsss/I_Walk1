using UGESystem;
using UnityEngine;
using TMPro;

namespace I_Walk
{
    public class NPCEmotions : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _tmpGUI;
        
        Animator _anim;

        void Start()
        {
            _anim = GetComponent<Animator>();
        }

        void Update()
        {
            if (_tmpGUI.text == "평소에는 안늦더니...어쩌다가 늦었니?")
            {
                //애니메이션이 반복재생됨 => 한번만 재생되도록 수정
                _anim.SetTrigger("Cocky");
            }
        }
    }
}
