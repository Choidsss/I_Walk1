using TMPro;
using UnityEngine;

namespace I_Walk
{
    public class NPCExpression : MonoBehaviour
    {
        //지금이 몇신데 이제오니?
        [SerializeField] TextMeshProUGUI _tmpGUI;

        Animator _anim;

        void Start()
        {
            _anim = GetComponent<Animator>();
        }

        void Update()
        {
            if (_tmpGUI.text == "지금이 몇신데 이제오니?")
            {
                //애니메이션이 반복재생됨 => 한번만 재생되도록 수정
                _anim.SetTrigger("Cocky");
            }
        }
    }
}
