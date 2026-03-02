using TMPro;
using UnityEngine;

namespace I_Walk
{
    public class NPCExpression : MonoBehaviour
    {
        Animator _anim;

        void Start()
        {
            _anim = GetComponent<Animator>();
        }

        void Update()
        {
            TeacherTalkExpression();
            FriendExpression();
        }

        public void TeacherTalkExpression()
        {
            TextMeshProUGUI TmpPro = GetComponentInChildren<TextMeshProUGUI>();

            if (TmpPro != null && TmpPro.text == "지금이 몇신데 이제오니?")
            {
                //애니메이션이 반복재생됨 => 한번만 재생되도록 수정
                _anim.SetTrigger("Cocky");
            }
            else
            {
                return;
            }
        }

        public void FriendExpression()
        {
            TextMeshProUGUI TmpPro = GetComponentInChildren<TextMeshProUGUI>();

            if (TmpPro != null && TmpPro.text == "ㅈㄴ 뛸 준비 해")
            {
                //애니메이션이 반복재생됨 => 한번만 재생되도록 수정
                _anim.SetTrigger("Hard");
            }
            else
            {
                return;
            }
        }
    }
}
