using UnityEngine;
using MxM;

namespace I_Walk
{
    public class MxMQuickTest : MonoBehaviour
    {
        private MxMTrajectoryGenerator targetGenerator;

        void Start() => targetGenerator = GetComponent<MxMTrajectoryGenerator>();

        void Update()
        {
            // WASD 입력을 벡터로 변환
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

            // MxM에게 입력값 전달
            targetGenerator.InputVector = input;
        }
    }
}
