using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Scene.TacticsBattle
{
    public class HpSliderController : MonoBehaviour
    {
        public Slider Slider = null;
        public Image FrontImage = null;
        public Image BackImage = null;

        Coroutine animation;

        void Start()
        {
            this.Slider.value = 0f;
            animation = null;
        }

        public void SetValue(float f)
        {
            if (animation != null)
            {
                StopCoroutine(animation);
            }

            animation = StartCoroutine(CoAnimHp(Slider.value, f, 1f));
        }

        IEnumerator CoAnimHp(float start, float end, float second)
        {
            float timePrev = 0f;
            float timeAcc = 0f;

            while (timeAcc < second)
            {
                float fraction = timeAcc / second;
                this.Slider.value = Mathf.Lerp(start, end, fraction);

                yield return null;

                if (timePrev > 0f)
                {
                    timeAcc += Time.time - timePrev;
                }

                timePrev = Time.time;
            }

            if (end == 0f)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
