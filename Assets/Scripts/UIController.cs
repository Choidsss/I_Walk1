using UnityEngine;
using UGESystem;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace I_Walk
{
    public class UIController : MonoBehaviour
    {


        private void Start()
        {
            
        }


        private void OnTriggerEnter(Collider other)
        {
            FirstScreenEffect();
        }

        // ToDo : 중간에 암전이 되도록 함
        private void FirstScreenEffect()
        {
                
        }
    }
}
