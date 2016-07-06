using UnityEngine;
using System.Collections;

/// <summary>
/// TODO 引数3つに直す
/// </summary>
namespace Basic
{
    public static class Easing
    {
        //public delegate float Function(float t);
        public delegate float Function(float start, float end, float time);

        public static float Liner3(float start, float end, float time)
        {
            return (end - start) * time + start;
        }

        static float Fix(float t)
        {
            return Mathf.Max(0f, Mathf.Min(1f, t));
        }

        /// <summary>
        /// From gems 4 p94 XXX
        /// </summary>
        public static float SCurve(float t)
        {
            var fix = Fix(t);

            return Mathf.Pow(fix, 5) * 6
                + Mathf.Pow(fix, 4) * 15
                - Mathf.Pow(fix, 3) * 10;
        }

        public static float Liner(float t)
        {
            return Fix(t);
        }


        #region === Sine ===

        public static float SineIn(float time)
        {
            return -1f * Mathf.Cos(time * Mathf.PI / 2f) + 1f;
        }

        public static float SineOut(float time)
        {
            return Mathf.Sin(time * Mathf.PI / 2f);
        }

        public static float SineInOut(float time)
        {
            return -0.5f * (Mathf.Cos(Mathf.PI * time) - 1f);
        }

        #endregion

        #region === Quad ===

        public static float QuadIn(float time)
        {
            return time * time;
        }

        public static float QuadOut(float time)
        {
            return -1 * time * (time - 2);
        }

        public static float QuadInOut(float time)
        {
            time = time * 2;
            if (time < 1)
                return 0.5f * time * time;
            --time;
            return -0.5f * (time * (time - 2) - 1);
        }

        #endregion
    }
}
