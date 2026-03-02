using UnityEngine;

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

        [Header("Interaction Settings")]
        [Tooltip("Maximum distance at which interaction will be detected")]
        [SerializeField] private float _interactionDistance = 3f;

        [Header("HitLayer")]
        [SerializeField] private LayerMask _layer;

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
            RaycastHit hit;

            Vector3 RayOrigin = transform.position;
            Vector3 RayDirection = transform.forward;

            if (Physics.Raycast(RayOrigin, RayDirection, _interactionDistance, _layer))
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
