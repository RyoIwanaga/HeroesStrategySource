using UnityEngine;
using System.Collections;

namespace UnityBehaviour.Camera
{
    public class LinerCameraBehaviour : MonoBehaviour
    {
        public Vector3 AddPosition = new Vector3(0f, 0f, 0f);
        public Vector3 AddEulerAngle = new Vector3(0f, 0f, 0f);

        void LateUpdate()
        {
            transform.position += AddPosition * Time.fixedDeltaTime;
            transform.eulerAngles += AddEulerAngle * Time.fixedDeltaTime;
        }
    }
}
