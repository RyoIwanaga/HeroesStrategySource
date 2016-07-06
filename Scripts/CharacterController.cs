using UnityEngine;
using System.Collections;



namespace HackSlash
{
    /*
    using AnimationController = CharacterAnimationController;

    public class CharacterController : MonoBehaviour
    {
        float Speed = 10;


        bool isLockingMovement = false;

        void Awake()
        {
            Debug.Log(DebugUtil.FN);
        }

        void Start()
        {
            ActionCharactor = GetComponent<ActionCharactorBehaviour>();
            Animation = GetComponent<CharacterAnimationController>();
        }


        public void Move(float fraction, Vector3 vecUnit)
        {
//            Debug.Log(DebugUtil.FN);
            Debug.Assert(Basic.Util.FloatSameValue(vecUnit.sqrMagnitude, 1f));

            Animation.SetSpeed(fraction);
            ActionCharactor.Move(Speed, vecUnit);
        }

        public void Stop()
        {
//            Debug.Log(DebugUtil.FN);

            Animation.SetSpeed(0f);
        }

        public void Attack()
        {
            Debug.Log(DebugUtil.FN);
            isLockingMovement = true;

//            var newState = new AnimationController.AttackingAnimationState();
//            newState.OnExitFinishCallback = () => _IsLockingMovement = false;

//            _animation.FSM.ChangeState(newState);
            Animation.Attack(() => isLockingMovement = false);
        }
    } */
}

