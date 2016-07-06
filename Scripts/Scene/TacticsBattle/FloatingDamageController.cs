using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Scene.TacticsBattle
{
    public class FloatingDamageController : MonoBehaviour
    {
        const float SECOND_FADE = 2f;
        static Vector3 ADD_START_POS = new Vector3(0f, 2f, 0f);
        static Vector3 VECTOR_MOVE = new Vector3(0f, 30f, 0f);

        [SerializeField]
        Text back = null;
        [SerializeField]
        Text front = null;

        Coroutine_.Sequencer coSeq;

        Color colorBackStart;
        Color colorBackEnd;
        Color colorFrontStart;
        Color colorFrontEnd;
        Vector3 startPos;
        Vector3 goalPos;

        public void Init(int damage, Vector3 worldPosition)
        { 
            coSeq = new Coroutine_.Sequencer(this);

            string str = damage.ToString();
            back.text = front.text = str;

            var uiPos = UnityBasic.Util.Vec3XYOnly(Camera.main.WorldToScreenPoint(worldPosition + ADD_START_POS));

            colorBackStart = back.color;
            colorBackEnd = UnityBasic.Util.CreateInvisibleColor(colorBackStart);
            colorFrontStart = front.color;
            colorFrontEnd = UnityBasic.Util.CreateInvisibleColor(colorFrontStart);

            this.transform.position = uiPos;
            startPos = uiPos;
            goalPos = startPos + VECTOR_MOVE;

            coSeq.Add(Coroutine_.Action.LerpScale(
                this.gameObject,
                new Vector3(0.8f, 0.8f, 0.8f),
                new Vector3(1f, 1f, 1f),
                0.2f));
            coSeq.Add(Coroutine_.Action.CustomLerpOld(SECOND_FADE, FadeOutAndMove));
            coSeq.Add(Coroutine_.Action.Func(() => GameObject.Destroy(this.gameObject)));
        }

        void FadeOutAndMove(float t)
        {
            // fade out
            back.color = Color.Lerp(colorBackStart, colorBackEnd, t);
            front.color = Color.Lerp(colorFrontStart, colorFrontEnd, t);

            // move
            transform.position = Vector3.Lerp(startPos, goalPos, t);

            // scale
            var fixedScale = Mathf.Lerp(1f, 0.8f, t);
            transform.localScale = new Vector3(fixedScale, fixedScale, fixedScale);
        }
    }
}
