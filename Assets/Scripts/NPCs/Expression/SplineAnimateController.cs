using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using TMPro;

namespace I_Walk
{
    public class SplineAnimateController : MonoBehaviour
    {
        [Header("PlayKeyward")]
        [SerializeField] string _playKeyward;
        
        Animator _anim;
        SplineAnimate _splineAnimate;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _anim = GetComponent<Animator>();
            _splineAnimate = GetComponent<SplineAnimate>();
        }

        // Update is called once per frame
        void Update()
        {
            TextMeshProUGUI tmpUGUI = GetComponentInChildren<TextMeshProUGUI>();

            if (tmpUGUI.text == _playKeyward)
            {
                SplineAnimationPlayOn();
            }
            else
            {
                return;
            }

            if (_splineAnimate.NormalizedTime >= 1f)
            {
                SplineAnimationEnded();
            }
        }

        private void SplineAnimationPlayOn()
        {
            //ToDo : 딜레이 3초 주기
            _splineAnimate.Play();
            _anim.SetBool("Running", true);
        }

        private void SplineAnimationEnded()
        {
            _anim.SetBool("Running", false);
        }

        //IEnumerator DelayAnimation()
        //{
        //    yield return 1;
        //}
    }
}
