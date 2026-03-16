using UnityEngine;

namespace I_Walk
{
    public class AnimationSpeedChanger : MonoBehaviour
    {
        [SerializeField] GameObject _player;

        private void OnTriggerEnter(Collider other)
        {
            PlayerFastMove fast = _player.GetComponent<PlayerFastMove>();

            if(fast != null)
            {
                fast.Speed = 2.0f;
            }
            else
            {
                //**********언제 돌아오게 만들건데????????**********
            }
        }
    }
}
