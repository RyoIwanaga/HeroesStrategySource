using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Basic;

namespace Scene.TacticsBattle
{
    public class TargetImageController : MonoBehaviour
    {
        const float SECOND_FADE_IN = 1.2f;
        const float SECOND_FADE_OUT = 1.2f;
        static Vector3 TARGET_BUTTON_POS = new Vector3(0f, 0.1f, 0f);

        public delegate Vector3 GetDrawingPosFunction();
        GetDrawingPosFunction getDrawingPosFunction = null;

        const float buttonSize = 15f;
        const float buttonSizeSqr = buttonSize * buttonSize;
        static Color colorStart = new Color(200f / 255f, 200f / 255f, 200f / 255f, 0f);
        static Color colorEnd = new Color(200f / 255f, 200f / 255f, 200f / 255f, 200f / 255f);

        RectTransform rect = null;
        Image image = null;


        public void Init(string spriteName, GetDrawingPosFunction function)
        {
            this.image = GetComponent<Image>();
            this.rect = GetComponent<RectTransform>();
            getDrawingPosFunction = function;

            GetComponent<Image>().sprite = ResourceManager.Instance.LoadSprite(spriteName);

            StartCoroutine(CoFadeLoop());
        }

        void LateUpdate()
        {
            var worldPos = getDrawingPosFunction() + TARGET_BUTTON_POS;
            var UIPos = Camera.main.WorldToScreenPoint(worldPos);
            var camPos = Camera.main.transform.position;
            var vec = worldPos - camPos;
            var distanceSqr = vec.sqrMagnitude;

            var fixedSize = buttonSizeSqr / distanceSqr;

            this.rect.sizeDelta = new Vector2(buttonSize, buttonSize);
            this.transform.position = UnityBasic.Util.Vec3XYOnly(UIPos);
        }

        IEnumerator CoFadeLoop()
        {
            for (;;)
            {
                yield return Coroutine_.Action.LerpColor(
                    this.image,
                    colorStart,
                    colorEnd,
                    SECOND_FADE_IN);

                yield return Coroutine_.Action.LerpColor(
                    this.image,
                    colorEnd,
                    colorStart,
                    SECOND_FADE_OUT);
            }
        }
    }
}
