using UnityEngine;

namespace I_Walk
{
    public class GameSounds : MonoBehaviour
    {
        [Header("AudioClips")]
        //[SerializeField] AudioClip _gameMenuSound;
        [SerializeField] AudioClip _inGameBackgroundSound; // 아직 안넣음,  null인상태
        [SerializeField] AudioClip _clickSound; //클릭하는 소리는 이거로 퉁치기

        [Header("AudioSource")]
        [SerializeField]AudioSource _audioSource;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void ClickSound()
        {
            if (_clickSound != null)
            {
                _audioSource.PlayOneShot(_clickSound);
            }
        }

        public void GameMenuUISoundStart()
        {
            if(_audioSource != null)
            {
                _audioSource.Play();
            }
        }

        public void GameMenuUISoundStop()
        {
            _audioSource.Stop();
        }
    }
}
