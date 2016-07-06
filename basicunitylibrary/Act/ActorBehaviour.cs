using UnityEngine;
using System.Collections;

namespace Act
{
    public abstract class ActorBehaviourBase : MonoBehaviour
    {
        Actor _actor;
        public Actor Actor
        {
            get { return _actor; }
        }

        public void Init()
        {
            _actor = new Actor(this);
        }
    }

    public class ActorUpdateBehaviour : ActorBehaviourBase
    {
        public void Update()
        {
            Actor.Apply();
        }
    }

    public class ActorLateUpdateBehaviour : ActorBehaviourBase
    {
        public void LateUpdate()
        {
            Actor.Apply();
        }
    }

    public class ActorFixedUpdateBehaviour : ActorBehaviourBase
    {
        public void FixedUpdate()
        {
            Actor.Apply();
        }
    }
}
