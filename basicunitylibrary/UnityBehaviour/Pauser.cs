using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityBehaviour
{
    /// <summary>
    /// Pause all behaviour.
    /// TODO: stop coroutine
    /// </summary>
    public class Pauser : MonoBehaviour
    {
        static LinkedList<Pauser> targets = new LinkedList<Pauser>();

        Behaviour[] behaviours = null;

        void Start()
        {
            targets.AddFirst(this);
        }

        void OnDestroy()
        {
            targets.Remove(this);
        }

        void OnPause()
        {
            if (behaviours != null)
            {
                return;
            }

            behaviours = Array.FindAll(GetComponentsInChildren<Behaviour>(), (obj) => { return obj.enabled; });

            foreach (var com in behaviours)
            {
                com.enabled = false;
            }
        }

        void OnResume()
        {
            if (behaviours == null)
            {
                return;
            }

            foreach (var com in behaviours)
            {
                com.enabled = true;
            }

            behaviours = null;
        }


        public static void Pause()
        {
            foreach (var item in targets)
            {
                item.OnPause();
            }
        }

        public static void Resume()
        {
            foreach (var item in targets)
            {
                item.OnResume();
            }
        }
    }
}
