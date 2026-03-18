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

        float _defaultY;
        bool _isPlayer = false;
        Vector3 _localPos;

        void Start()
        {
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
            else
            {
                _QMark.SetActive(true);
            }
        }


        void CheckShowBangMark()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            TextMeshProUGUI IsText = GetComponentInChildren<TextMeshProUGUI>();

            if (IsText.text != null)
            {
                _isPlayer = true;
            }
            else
            {
                _isPlayer = false;
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
