using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BoardGame;

using Basic;

namespace TacticsBG
{
    public class AttackDirectionController : MonoBehaviour
    {
        const float WAIT = 1.2f;
        const float SECOND = 1.2f;
        const float HEIGHT = 0.2f;

        static Color colorStart = new Color(200f / 255f, 200f / 255f, 200f / 255f, 0f);
        static Color colorEnd = new Color(200f / 255f, 200f / 255f, 200f / 255f, 200f / 255f);

        Image image = null;

        Vector3 start;
        Vector3 end;

        public void Init(Basic.Vec2Int pos, Basic.Vec2Int targetPos, System.Func<Basic.Vec2Int, Vector3> fnConvertPos)
        {
            image = GetComponent<Image>();

            // start
            var rotateY = -45f;
            var vec = targetPos - pos;
            var degree = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;

            var gridSize = fnConvertPos(new Basic.Vec2Int(1, 1));

            this.transform.eulerAngles -= new Vector3(0f, rotateY + degree, 0f);


            var vec3Fixed = new Vector3(vec.x * 0.1f, 0f, vec.y * 0.1f);
            var pos3 = fnConvertPos(pos);
            pos3.y = HEIGHT;

            start = pos3 - vec3Fixed;
            end = pos3 + vec3Fixed;

            StartCoroutine(CoFadeInAndMove());
        }

        void dosomthing(float f)
        {
            transform.position = Vector3.Lerp(start, end, Basic.Easing.SineOut(f));
            image.color = Color.Lerp(colorStart, colorEnd, f);
        }

        IEnumerator CoFadeInAndMove()
        {
            for (;;)
            {
                yield return Coroutine_.Action.CustomLerpOld(SECOND, this.dosomthing);
                yield return Coroutine_.Action.LerpColor(this.image, colorEnd, colorStart, SECOND);
            }
        }
    }
}
