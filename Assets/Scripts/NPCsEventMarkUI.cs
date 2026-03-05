using UnityEngine;
using TMPro;
using UGESystem;

namespace I_Walk
{
    public class NPCsEventMarkUI : MonoBehaviour
    {
        [Header("QuestionMark Animation Settings")]
        [SerializeField] float _rotationSpeed = 1.0f;
        [SerializeField] float _hopSpeed = 1.0f;
        [SerializeField] float _hopAmound = 1.0f;

        [Header("QuestionMark")]
        [SerializeField] GameObject _QMark;

        [SerializeField] private LayerMask _layer;

        [SerializeField] private float _interactionRange = 5f;
        
        float _defaultY;
        bool _isPlayer = false;
        Vector3 _localPos;

        void Start()
        {
            if (_QMark == null)
            {
                Debug.Log("이모지가 존재하지 않습니다");
            }

            _localPos = _QMark.transform.localPosition;
            _defaultY = _localPos.y;

            _QMark.SetActive(true);   
        }

        // Update is called once per frame
        void Update()
        {
            CheckShowBangMark();

            if (_isPlayer)
            {
                _QMark.SetActive(false);
            }
        }


        void CheckShowBangMark()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.SphereCast(ray , _interactionRange) && Input.GetKey(KeyCode.F))
            {
                _isPlayer = true;
            }
            BangMarkAnimation();
        }

        void BangMarkAnimation()
        {
            _QMark.transform.Rotate(Vector3.up * _rotationSpeed * Time.deltaTime);

            float bounce = Mathf.Abs(Mathf.Sin(Time.time * _hopSpeed)) * _hopAmound;
            float posY = _defaultY + bounce;

            _QMark.transform.localPosition = new Vector3(_localPos.x, posY, _localPos.z);
        }
    }
}
