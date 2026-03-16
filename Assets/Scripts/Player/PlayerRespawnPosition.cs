using System.Collections;
using UnityEngine;

namespace I_Walk
{
    public class PlayerRespawnPosition : MonoBehaviour
    {
        // 인스펙터에서 끌어다 놓을 '부활 위치' (빈 게임 오브젝트를 만들어두고 넣으면 편해!)
        [SerializeField] private Transform _respawnPoint;
        [SerializeField] GameObject _player;

        private void OnTriggerEnter(Collider other)
        {
            if (other.name == "Player")
            {
                StartRespawnSequence();
            }
        }
        

        // 트리거에 닿거나 특정 상황이 되면 이 함수를 불러와!
        public void StartRespawnSequence()
        {
            // 코루틴(시간 지연 기능) 시작
            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            
            // 1. 여기서 3초 동안 멈춰서 기다림
            yield return new WaitForSeconds(3f);

            // 2. 플레이어를 잠깐 꺼서(지워서) 화면에서 안 보이게 함
            _player.SetActive(false);

            // 3. 인스펙터에 넣어둔 지정 위치로 순간이동!
            _player.transform.position = _respawnPoint.position;

            // (선택) 만약 부활할 때 바라보는 방향도 지정한 곳과 맞추고 싶다면 아래 줄의 주석(//)을 지워줘
            // transform.rotation = _respawnPoint.rotation;

            // 4. 플레이어를 다시 켜서(생성해서) 짠! 하고 나타나게 함
            _player.SetActive(true);
        }
    }
}
