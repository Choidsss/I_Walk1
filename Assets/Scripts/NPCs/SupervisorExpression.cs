using TMPro;
using UnityEngine;

namespace I_Walk
{
    public class SupervisorExpression : BaseNPC
    {
        Animator _anim;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            ExpressEmotions();
        }

        public override void ExpressEmotions()
        {
            TextMeshProUGUI TmpPro = GetComponentInChildren<TextMeshProUGUI>();

            
            if (TmpPro != null && TmpPro.text == "장난하니?")
            {
                //애니메이션이 반복재생됨 => 한번만 재생되도록 수정
                _anim.SetTrigger("Anoyed");
            }
            else
            {
                return;
            }
        }
    }
}
