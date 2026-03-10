using TMPro;
using UnityEngine;

namespace I_Walk
{
    public class FriendExpression : BaseNPC
    {
        Animator _anim;

        string _lastEmotionText = "";
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

            if (TmpPro == null) return;

            if (TmpPro != null && TmpPro.text == "ㅈㄴ 뛸 준비 해")
            {
                //애니메이션이 반복재생됨 => 한번만 재생되도록 수정
                _anim.SetTrigger("Hard");
                _lastEmotionText = TmpPro.text;
            }
            else
            {
                return;
            }
        }
    }
}
