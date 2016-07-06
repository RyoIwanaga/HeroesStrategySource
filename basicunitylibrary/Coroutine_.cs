using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Coroutine_
{
    public class Node
    {
        protected Node()
        {
        }

        public static Node[] Convert(params object[] objs)
        {
            var nodes = new Node[objs.Length];

            for (int i = 0; i < objs.Length; i++)
            {
                {
                    var cast = objs[i] as IEnumerator;
                    if (cast != null)
                    {
                        nodes[i] = new NodeCoroutine(cast);
                        continue;
                    }
                }

                {
                    var cast = objs[i] as Spawn;
                    if (cast != null)
                    {
                        nodes[i] = cast;
                        continue;
                    }
                }

                {
                    var cast = objs[i] as Sequence;
                    if (cast != null)
                    {
                        nodes[i] = cast;
                        continue;
                    }
                }
            }

            return nodes;
        }
    }

    public class NodeCoroutine : Node
    {
        IEnumerator item;

        public NodeCoroutine(IEnumerator item) : base()
        {
            this.item = item;
        }
    }

    public class Spawn : Node
    {
        public Node[] nodes;

        Spawn(Node[] nodes) : base()
        {
            this.nodes = nodes;
        }

        public static Spawn Create(params object[] objs)
        {
            return new Spawn(Node.Convert(objs));
        }
    }

    public class Sequence : Node
    {
        public Node[] nodes;

        Sequence(Node[] nodes) : base()
        {
            this.nodes = nodes;
        }

        public static Sequence Create(params object[] objs)
        {
            return new Sequence(Node.Convert(objs));
        }
    }

    /// <summary>
    /// Coroutine を順次実行する
    /// </summary>
    public class Sequencer
    {
        List<IEnumerator> stack;
        MonoBehaviour parent;
        Coroutine executing;

        bool _isRunning;
        public bool IsRunning { get { return _isRunning; } }


        public Sequencer(MonoBehaviour parent)
        {
            Debug.Assert(parent != null);

            this.stack = new List<IEnumerator>();
            this.parent = parent;
            this._isRunning = false;
            executing = null;
        }

        public void StopCurrent()
        {
            if (executing != null)
            {
                parent.StopCoroutine(executing);
                executing = null;
                _isRunning = false;
            }
        }

        public void Add(IEnumerator co)
        {
            if (_isRunning == false)
            {
                executing = parent.StartCoroutine(Wait(co));
            }
            else
            {
                this.stack.Add(co);
            }
        }

        public void Add(IEnumerator[] coroutines, float secondExit = 10f)
        {
            this.Add(WaitSeq(coroutines, secondExit));
        }

        public void Add(Spawn spawn)
        {
            //
        }

        IEnumerator Wait(IEnumerator co)
        {
            this._isRunning = true;
            yield return co;
            this._isRunning = false;

            if (stack.Count >= 1)
            {
                parent.StartCoroutine(Wait(stack[0]));
                stack.RemoveAt(0);
            }
        }

        IEnumerator WaitSeq(IEnumerator[] coroutines, float secondExit = 10f)
        {
            var start = Time.time;
            var parent = this.parent;

            List<Sequencer> stacks = new List<Sequencer>();
            foreach (var co in coroutines)
            {
                var newStack = new Sequencer(this.parent);
                stacks.Add(newStack);

                newStack.Add(co);
            }

            // Are all coroutines stopping ?
            System.Func<Sequencer, bool> predicate = (Sequencer cs) => ! cs.IsRunning;

            for (;;)
            {
                if (Basic.Sequence.Every(stacks, predicate) 
                    || (Time.time - start >= secondExit))
                    break;
                
                yield return null;
            }
        }
    }

    public static class Action
    {
        public delegate void LeapFunction(float t);
        public delegate float EaseFunction(float t);

        public static IEnumerator Func(System.Action func)
        {
            func();

            yield return null;
        }

        public static IEnumerator Wait(float second, System.Action fn)
        {
            yield return new WaitForSeconds(second);
        }

        public static IEnumerator WaitFunc(float second, System.Action func)
        {
            yield return new WaitForSeconds(second);

            func();
        }

        public static IEnumerator CustomLerpOld(float second, LeapFunction lerpFunc, EaseFunction easeFunc = null, bool isReverse = false)
        {
            float timePrev = 0f;
            float timeAcc = 0f;

            while (timeAcc < second)
            {
                float time = timeAcc / second;
                if (isReverse)
                {
                    time = 1f - time;
                }

                // XXX リバース状態で　easing した時の値がおかしいかも
                lerpFunc(easeFunc == null ? Basic.Easing.Liner(time) : easeFunc(time));

                yield return null;

                if (timePrev > 0f)
                {
                    timeAcc += Time.time - timePrev;
                }

                timePrev = Time.time;
            }

            lerpFunc(isReverse ? 0f : 1f);
        }

        public delegate void LerpFunc<T>(T start, T end, float t);

        public static IEnumerator CustomLerp<T>(float second, T start, T end,
            LerpFunc<T> lerpFunc,
            EaseFunction easeFunc = null)
        {
            float timePrev = 0f;
            float timeAcc = 0f;

            while (timeAcc < second)
            {
                float time = timeAcc / second;

                if (easeFunc != null)
                    time = easeFunc(time);

                lerpFunc(start, end, time);

                yield return null;

                if (timePrev > 0f)
                {
                    timeAcc += Time.time - timePrev;
                }

                timePrev = Time.time;
            }
            
            lerpFunc(start, end, 1f);
        }

        public static IEnumerator CustomLerpReverse(float second, LeapFunction lerpFunc, EaseFunction easeFunc = null)
        {
            yield return CustomLerpOld(second, lerpFunc, easeFunc, true);
        }

        /// <summary>
        /// TODO 移動したほうがいいかも あまり汎用的じゃない
        /// </summary>
        /// <param name="coFadeOut"></param>
        /// <param name="coFadeIn"></param>
        /// <param name="fnMiddle"></param>
        /// <returns></returns>
        public static IEnumerator CrossFading(
            IEnumerator coFadeOut,
            IEnumerator coFadeIn,
            System.Action fnMiddle)
        {
            yield return coFadeOut;

            fnMiddle();

            yield return coFadeIn;
        }

        public static IEnumerator LerpColor(Image image, Color start, Color end, float second, System.Func<float, float> easeFn = null)
        {
            float timePrev = 0f;
            float timeAcc = 0f;

            while (timeAcc < second)
            {
                float time = timeAcc / second;
                float easingTime = easeFn == null ? Basic.Easing.Liner(time) : easeFn(time);
                image.color = Color.Lerp(start, end, easingTime);

                yield return null;

                if (timePrev > 0f)
                {
                    timeAcc += Time.time - timePrev;
                }

                timePrev = Time.time;
            }

            image.color = end;
        }

        public static IEnumerator LerpMove(GameObject obj, Vector3 start, Vector3 end, float speed, System.Func<float, float> easeFn = null)
        {
            float timePrev = 0f;
            float timeAcc = 0f;
            float distance = (end - start).magnitude;
            float timeNeed = distance / speed;

            while (timeAcc < timeNeed)
            {
                float time = timeAcc / timeNeed;
                float easingTime = easeFn == null ? Basic.Easing.Liner(time) : easeFn(time);
                obj.transform.position = Vector3.Lerp(start, end, easingTime);

                yield return null;

                if (timePrev > 0f)
                {
                    timeAcc += Time.time - timePrev;
                }

                timePrev = Time.time;
            }

            obj.transform.position = end;
        }

        public static IEnumerator LerpScale(GameObject obj, Vector3 startScale, Vector3 endScale, float second, System.Func<float, float> easeFn = null)
        {
            float timePrev = 0f;
            float timeAcc = 0f;

            while (timeAcc < second)
            {
                float time = timeAcc / second;
                float easingTime = easeFn == null ? Basic.Easing.Liner(time) : easeFn(time);

                obj.transform.localScale = new Vector3(
                    Mathf.Lerp(startScale.x, endScale.x, easingTime),
                    Mathf.Lerp(startScale.y, endScale.y, easingTime),
                    Mathf.Lerp(startScale.z, endScale.z, easingTime));

                yield return null;

                if (timePrev > 0f)
                {
                    timeAcc += Time.time - timePrev;
                }

                timePrev = Time.time;
            }

            obj.transform.localScale = endScale;
        }
    }
}
