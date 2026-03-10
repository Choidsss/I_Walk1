using UnityEngine;
using TMPro;

namespace I_Walk
{
    public class TeacherExpression : BaseNPC
    {
        Animator _anim;

        string _lastEmotionText = "";

        void Start()
        {
            _anim = GetComponent<Animator>();
        }

        void Update()
        {
            ExpressEmotions();
        }

        public override void ExpressEmotions()
        {
            TextMeshProUGUI TMPro = GetComponentInChildren<TextMeshProUGUI>();

            if (TMPro.text == _lastEmotionText) return;

            if (TMPro != null && TMPro.text == "지금이 몇신데 이제오니?")
            {
                //애니메이션이 반복재생됨 => 한번만 재생되도록 수정
                _anim.SetTrigger("Cocky");
                _lastEmotionText = TMPro.text;
            }
            else if (TMPro != null && TMPro.text == "그래, 얼른 들어가라")
            {
                _anim.SetTrigger("Dissmiss");
                _lastEmotionText = TMPro.text;
            }
            else
            {
                return;
            }


        }
    }
}
