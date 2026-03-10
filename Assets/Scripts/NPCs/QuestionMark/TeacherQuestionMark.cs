using UnityEngine;
using TMPro;
using static UnityEditorInternal.ReorderableList;

namespace I_Walk
{
    public class TeacherQuestionMark : MonoBehaviour
    {
        [SerializeField] GameObject _qMark;
        [SerializeField] string _text;

        [SerializeField] float _rotationSpeed = 1.0f;
        [SerializeField] float _hopSpeed = 1.0f;
        [SerializeField] float _hopAmound = 1.0f;

        float _defaultY;
        Vector3 _localPos;

        void Start()
        {
            _localPos = _qMark.transform.localPosition;
            _defaultY = _localPos.y;

            _qMark.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
            QuestionMarkDestroy();
        }

        void QuestionMarkDestroy()
        {
            TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();

            if (text.text == _text)
            {
                Destroy(_qMark);
            }

            BangMarkAnimation();
        }

        void BangMarkAnimation()
        {
            _qMark.transform.Rotate(Vector3.up * _rotationSpeed * Time.deltaTime);

            float bounce = Mathf.Abs(Mathf.Sin(Time.time * _hopSpeed)) * _hopAmound;
            float posY = _defaultY + bounce;

            _qMark.transform.localPosition = new Vector3(_localPos.x, posY, _localPos.z);
        }
    }
}
