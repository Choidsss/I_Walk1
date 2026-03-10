using TMPro;
using UnityEngine;

namespace I_Walk
{
    public class ComManagerExpression : BaseNPC
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

            //애니메이션 1 => 절레절레
            //애니메이션 2 => 손가락으로 짚는 애니메이션
            if (TmpPro != null && TmpPro.text == "지금부터 본인이 이제부터 해야되는 일들을 알려드릴게요.")
            {
                //애니메이션이 반복재생됨 => 한번만 재생되도록 수정
                _anim.SetTrigger("Point");
                _lastEmotionText = TmpPro.text;
            }
            else if (TmpPro != null && TmpPro.text == "그리고 여기 팀장님이 일을 잘 안하시는 편이거든요.")
            {
                //애니메이션이 반복재생됨 => 한번만 재생되도록 수정
                _anim.SetTrigger("Annoyed");
                _lastEmotionText = TmpPro.text;
            }
            else if (TmpPro != null && TmpPro.text == "다니시다 보면 느끼실거에요, 제가 무슨 말 하는지.")
            {
                //애니메이션이 반복재생됨 => 한번만 재생되도록 수정
                _anim.SetTrigger("Annoyed");
                _lastEmotionText = TmpPro.text;
            }
            else
            {
                return;
            }
        }
    }
}
