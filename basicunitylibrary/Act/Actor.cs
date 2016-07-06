using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// iTween like library but can be use own updater :)
/// 
/// TODO stop timer spawn seq
/// </summary>
namespace Act
{
    public interface IActable
    {
        Actor Actor { get; }
    }

    public enum UpdateTiming
    {
        Update,
        LateUpdate,
        FixedUpdate,
    }

    public class Actor
    {
        #region === Static function ===

        public static void AttachRun(GameObject target, ActNode act, UpdateTiming updateTiming = UpdateTiming.Update)
        {
            ActorBehaviourBase behaviour;

            switch (updateTiming)
            {
                case UpdateTiming.Update:
                    behaviour = target.AddComponent<ActorUpdateBehaviour>();
                    behaviour.Init();
                    behaviour.Actor.Add(act);
                    break;
                case UpdateTiming.LateUpdate:
                    behaviour = target.AddComponent<ActorLateUpdateBehaviour>();
                    behaviour.Init();
                    behaviour.Actor.Add(act);
                    break;
                case UpdateTiming.FixedUpdate:
                    behaviour = target.AddComponent<ActorFixedUpdateBehaviour>();
                    behaviour.Init();
                    behaviour.Actor.Add(act);
                    break;
            }
        }

        #endregion

        #region === Property ===

        MonoBehaviour updater;
        List<ActNode> acts;
        int countCash;

        #endregion

        public Actor(MonoBehaviour updater)
        {
            this.updater = updater;

            this.acts = new List<ActNode>();
            countCash = 0;
        }

        public void Add(ActNode act)
        {
            this.acts.Add(act);

            countCash = acts.Count;
        }


        /// <summary>
        /// Apply action
        /// </summary>
        /// <returns>Return true if rest action is appliable.</returns>
        public bool Apply()
        {
            /* Apply action */

            if (countCash > 0)
            {

                var result = acts[0].Apply();

                if (result)
                {
                    acts.RemoveAt(0);
                    countCash = acts.Count;
                }
                return true;
            }

            /* Do anything */

            else
            {
                return false;
            }
        }

    }

    #region === Acts ===

    public abstract class ActNode
    {
        /// <summary>
        /// Apply action.
        /// </summary>
        /// <returns>Return true if end.</returns>
        public abstract bool Apply();

    }

    public abstract class ActBase : ActNode
    {
        /// <summary>
        /// This is called from Loop
        /// </summary>
        public abstract void Initialize();
    }

    public class LoopForever : ActNode
    {
        LinkedList<ActBase> actions;
        LinkedListNode<ActBase> currentNode;

        LoopForever(ActBase[] actions)
        {
            Debug.Assert(actions.Length >= 1);

            this.actions = new LinkedList<ActBase>(actions);
            this.currentNode = this.actions.First;
        }

        public static LoopForever Create(params ActBase[] actions)
        {
            Debug.Assert(actions.Length >= 1);

            return new LoopForever(actions);
        }

        public override bool Apply()
        {
            if (currentNode.Value.Apply())
            {
                // Initialize current action
                currentNode.Value.Initialize();

                // go next
                if (currentNode.Next == null)
                {
                    currentNode = actions.First;
                }
                else
                {
                    currentNode = currentNode.Next;
                }
            }

            // Never end :/
            return false;
        }
    }

    public abstract class LerpBase<T> : ActBase
    {
        float timePrevious = -1f;
        float timeAcc = 0f;
        float timeWait;

        T start;
        T end;

        Basic.Easing.Function _easeFunc;
        protected Basic.Easing.Function EaseFunc { get { return _easeFunc; } }

        protected LerpBase(T start, T end, float second, Basic.Easing.Function easeFunc = null)
        {
            this.start = start;
            this.end = end;
            this.timeWait = second;

            // use default or given function
            this._easeFunc = easeFunc == null ?
                Basic.Easing.Liner3 :
                easeFunc;
        }

        public abstract void ApplyLerp(T start, T end, float timeFrac);

        public override void Initialize()
        {
            this.timePrevious = Time.time;
            this.timeAcc = 0f;
        }

        // is finish?
        public override bool Apply()
        {
            /* Initialize */

            if (this.timePrevious < 0f)
            {
                this.Initialize();
            }

            /* Finish ? */

            if (this.timeAcc > this.timeWait)
            {
                // TODO Apply ease???
                ApplyLerp(start, end, 1f);

                return true;
            }

            /* Loop */

            else
            {
                var timeFrac = this.timeAcc / this.timeWait;
                ApplyLerp(start, end, timeFrac);

                this.timeAcc += Time.time - this.timePrevious;

                this.timePrevious = Time.time;

                return false;
            }
        }
    }

    public class LerpCustom<T> : LerpBase<T>
    {
        System.Action<T, T, float> lerpFunc;

        // Dont use easeFunc field

        LerpCustom(T start, T end, float second, 
            System.Action<T, T, float> lerpFunc) :
            base(start, end, second)
        {
            this.lerpFunc = lerpFunc;
        }

        public static LerpCustom<T> Create(T start, T end, float second, 
            System.Action<T, T, float> lerpFunc)
        {
            return new LerpCustom<T>(start, end, second, lerpFunc);
        }

        public override void ApplyLerp(T start, T end, float timeFrac)
        {
            lerpFunc(start, end, timeFrac);
        }
    }

    // TODO need test
    public class LerpSize : LerpBase<Vector3>
    {
        GameObject target;

        LerpSize(GameObject target, Vector2 start, Vector2 end, float second, Basic.Easing.Function easeFunc) :
            base(start, end, second, easeFunc)
        {
            this.target = target;
        }

        public static LerpSize Create(GameObject target, Vector2 start, Vector2 end, float second, Basic.Easing.Function easeFunc = null)
        {
            return new LerpSize(target, start, end, second, easeFunc);
        }

        public override void ApplyLerp(Vector3 start, Vector3 end, float timeFrac)
        {
            this.target.transform.localScale = new Vector3(
                EaseFunc(start.x, end.x, timeFrac),
                EaseFunc(start.y, end.y, timeFrac),
                EaseFunc(start.z, end.z, timeFrac));
        }
    }

    public class LerpRectSize : LerpBase<Vector2>
    {
        RectTransform target;

        LerpRectSize(RectTransform target, Vector2 start, Vector2 end, float second, Basic.Easing.Function easeFunc) :
            base(start, end, second, easeFunc)
        {
            this.target = target;
        }

        public static LerpRectSize Create(RectTransform target, Vector2 start, Vector2 end, float second, Basic.Easing.Function easeFunc = null)
        {
            return new LerpRectSize(target, start, end, second, easeFunc);
        }

        public override void ApplyLerp(Vector2 start, Vector2 end, float timeFrac)
        {
            target.sizeDelta = new Vector2(
                EaseFunc(start.x, end.x, timeFrac),
                EaseFunc(start.y, end.y, timeFrac));
        }
    }

	// TODO Text version
	public class LerpColor : LerpBase<Color>
	{
		Image target;
		
		LerpColor(Image target, Color start, Color end, float second, Basic.Easing.Function easeFunc) :
			base(start, end, second, easeFunc)
		{
			this.target = target;
		}
		
		public static LerpColor Create(Image target, Color start, Color end, float second, Basic.Easing.Function easeFunc = null)
		{
			return new LerpColor(target, start, end, second, easeFunc);
		}
		
		public override void ApplyLerp(Color start, Color end, float timeFrac)
		{
			this.target.color = new Color(
				EaseFunc(start.r, end.r, timeFrac),
				EaseFunc(start.g, end.g, timeFrac),
				EaseFunc(start.b, end.b, timeFrac),
				EaseFunc(start.a, end.a, timeFrac));
		}
	}

    public class WaitFunc : ActNode
    {
        float timePrevious = -1f;
        float timeAcc = 0f;
        float timeWait;

        System.Action onFinishFunc;

        WaitFunc(float second, System.Action onFinishFunc)
        {
            this.timeWait = second;
            this.onFinishFunc = onFinishFunc;
        }

        public static WaitFunc Create(float second, System.Action onFinishFunc)
        {
            return new WaitFunc(second, onFinishFunc);
        }

        // is finish?
        public override bool Apply()
        {
            /* Initialize */

            if (this.timePrevious < 0f)
            {
                this.timePrevious = Time.time;
                this.timeAcc = 0f;
            }

            /* Finish ? */

            if (this.timeAcc > this.timeWait)
            {
                this.onFinishFunc();

                return true;
            }

            /* Loop */

            else
            {

                this.timeAcc += Time.time - this.timePrevious;

                this.timePrevious = Time.time;

                return false;
            }
        }
    }

    #endregion
}
