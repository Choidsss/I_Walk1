using TMPro;
using UnityEngine;

namespace I_Walk
{
    public class RoomMate2Expression : BaseNPC
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

            if (TmpPro != null && TmpPro.text == "또 내려가?")
            {
                _anim.SetTrigger("HeadShake");
                _lastEmotionText = TmpPro.text;
            }
            else
            {
                return;
            }
        }
    }
}
