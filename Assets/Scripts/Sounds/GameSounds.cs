using UnityEngine;

namespace I_Walk
{
    public class GameSounds : MonoBehaviour
    {
        [SerializeField] AudioClip _walkSound;
        [SerializeField] AudioClip _failSound;
        [SerializeField] AudioClip _gameMenuSound;
        [SerializeField] AudioClip _inGameBackgroundSound;
        [SerializeField] AudioClip _clickSound; //클릭하는 소리는 이거로 퉁치기

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
