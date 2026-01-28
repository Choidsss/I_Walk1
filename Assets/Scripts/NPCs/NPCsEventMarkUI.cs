using UnityEngine;

namespace I_Walk
{
    public class NPCsEventMarkUI : MonoBehaviour
    {
        [Header("BangMark Animation Settings")]
        [SerializeField] float _rotationSpeed = 1.0f;
        [SerializeField] float _hopSpeed = 1.0f;
        [SerializeField] float _hopAmound = 1.0f;

        [Header("Bang Mark")]
        [SerializeField] GameObject _bangMark;

        NPCsDetectionRange _npcDetection;

        float _defaultY;
        Vector3 _localPos;

        // ***************ToDo : Y축 기준으로 돌리기, Y축의 좌표값을 높였다, 낮췄다 해보기****************
        void Start()
        {
            _npcDetection = GetComponent<NPCsDetectionRange>();

            if (_bangMark == null)
            {
                Debug.Log("이모지가 존재하지 않습니다");
            }

            _localPos = _bangMark.transform.localPosition;
            _defaultY = _localPos.y;

            _bangMark.SetActive(false);   
        }

        // Update is called once per frame
        void Update()
        {
            ShowBangMark();
        }


        void ShowBangMark()
        {
            if (_npcDetection == null)
            {
                Debug.Log("NPCsDetectionRange 스크립트가 존재하지 않습니다");
            }


            if(_npcDetection.IsPlayer == true)
            {
                _bangMark.SetActive(true);

                BangMarkAnimation();
            }
            else
            {
                _bangMark.SetActive(false);
            }
        }

        void BangMarkAnimation()
        {
            _bangMark.transform.Rotate(Vector3.up * _rotationSpeed * Time.deltaTime);

            float bounce = Mathf.Abs(Mathf.Sin(Time.time * _hopSpeed)) * _hopAmound;
            float posY = _defaultY + bounce;

            _bangMark.transform.localPosition = new Vector3(_localPos.x, posY, _localPos.z);
        }
    }
}
