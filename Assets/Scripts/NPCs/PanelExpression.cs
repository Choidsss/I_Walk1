using TMPro;
using UnityEngine;

namespace I_Walk
{
    public class PanelExpression : BaseNPC
    {
        [SerializeField] string _emotionAnim;

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

            if (TmpPro != null && TmpPro.text == _emotionAnim)
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
