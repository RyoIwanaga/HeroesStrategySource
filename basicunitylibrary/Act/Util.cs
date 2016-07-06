using UnityEngine;
using System.Collections;

namespace Act
{
    public static class Util
    {
        public static void EffectPlayOneShot(Object o, Vector3 pos, Quaternion dir, float waitSecond = 5f)
        {
            var go = GameObject.Instantiate(o, pos, dir) as GameObject;

            Act.Actor.AttachRun(go, Act.WaitFunc.Create(waitSecond, () => GameObject.Destroy(go)));
        }

        public static void EffectPlayOneShot(Object o, Vector3 pos, float waitSecond = 5f)
        {
            EffectPlayOneShot(o, pos, Quaternion.identity, waitSecond);
        }
    }
}
