using UnityEngine;
using System.Collections;

namespace UnityBehaviour
{
    public class ActionCharactorBehaviour : MonoBehaviour
    {
        CharacterController cc;

        void Start()
        {
            cc = GetComponent<CharacterController>();
        }

        public void Move(float speed, Vector3 vecUnit)
        {
            Debug.Assert(Basic.Util.FloatSameValue(vecUnit.sqrMagnitude, 1f));

            cc.Move(vecUnit * speed * Time.deltaTime);
            cc.Move(new Vector3(0f, -1f, 0f));

            this.transform.rotation = Quaternion.LookRotation(vecUnit);
        }
    }
}
