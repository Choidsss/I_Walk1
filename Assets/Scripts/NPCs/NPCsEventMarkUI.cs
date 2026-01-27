using UnityEngine;

namespace I_Walk
{
    public class NPCsEventMarkUI : MonoBehaviour
    {
        [SerializeField] GameObject _bangMark;

        bool _isPlayer = false;

        void Start()
        {
            _bangMark.SetActive(false);   
        }

        // Update is called once per frame
        void Update()
        {
            ShowBangMark();
        }

        void ShowBangMark()
        {
            if(_isPlayer == true)
            {
                _bangMark.SetActive(true);
            }
            else
            {
                _bangMark.SetActive(false);
            }
        }
    }
}
