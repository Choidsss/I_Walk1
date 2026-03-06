using TMPro;
using UnityEngine;

namespace I_Walk
{
    public class RoomMateExpression : BaseNPC
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

            if (TmpPro != null && TmpPro.text == "에휴...됐다....됐어...ㅅㅂ 말하면 뭐하냐")
            {
                //애니메이션이 반복재생됨 => 한번만 재생되도록 수정
                _anim.SetTrigger("PointForward");
            }
            if (TmpPro != null && TmpPro.text == "넌 뭐 우리가 부담스럽냐?")
            {
                //애니메이션이 반복재생됨 => 한번만 재생되도록 수정
                _anim.SetTrigger("Talking");
            }
            else
            {
                return;
            }
        }
    }
}
