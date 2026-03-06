using UnityEngine;
using TMPro;

namespace I_Walk
{
    public class TeacherExpression : BaseNPC
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
            TextMeshProUGUI TMPro = GetComponentInChildren<TextMeshProUGUI>();

            if (TMPro != null && TMPro.text == "지금이 몇신데 이제오니?")
            {
                //애니메이션이 반복재생됨 => 한번만 재생되도록 수정
                _anim.SetTrigger("Cocky");
            }
            else if (TMPro != null && TMPro.text == "그래, 얼른 들어가라")
            {
                //Dissmiss 제스처 => 절레절레하는 애니메이션으로 바꾸기
                _anim.SetTrigger("Dissmiss");
            }
            else
            {
                return;
            }

            //switch (_emotionAnim)
            //{
            //    case "지금이 몇신데 이제오니?":
            //        _anim.SetTrigger("Cocky");
            //        break;
            //    default:
            //        return;
            //}
        }
    }
}
