using TMPro;
using UnityEngine;

namespace I_Walk
{
    public class PanelExpression : BaseNPC
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

            if (TmpPro != null && TmpPro.text == "학점은 그래도 다 괜찮았네.")
            {
                //애니메이션이 반복재생됨 => 한번만 재생되도록 수정
                _anim.SetTrigger("Yes");
            }
            else if (TmpPro != null && TmpPro.text == "수고했네")
            {
                _anim.SetTrigger("Yes");
            }
            else
            {
                return;
            }
        }
    }
}
