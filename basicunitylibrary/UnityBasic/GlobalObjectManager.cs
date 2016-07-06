using UnityEngine;
using System.Collections;

namespace UnityBasic
{
    public class GlobalObjectManager : MonoBehaviour
    {
        [SerializeField]
        string TagString = "";

        bool IsFist = false;

        void Start()
        {
            Debug.Assert(TagString != "");

            var globalObjects = GameObject.FindGameObjectsWithTag(TagString);

            if (globalObjects == null)
            {
                Debug.Log(DebugUtil.FN + "Invalid TagString");
            }
            else if (globalObjects.Length == 1)
            {
                this.IsFist = true;
            }
            else if (globalObjects.Length == 2)
            {
                GameObject.Destroy(this.gameObject);
            }
            else
            {
                Debug.Assert(false);
            }
        }
    }
}
