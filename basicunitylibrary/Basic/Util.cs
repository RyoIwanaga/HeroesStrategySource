using System;
using System.Collections;

/// <summary>
/// Basic library 
/// </summary>
namespace Basic
{
    public static class Util
    {
        public static bool FloatSameValue(float target, float compare)
        {
            const float value = 0.001f;

            return System.Math.Abs(target - compare) < value;
        }

        public static int InRange(int n, int max, int min = 0)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(min < max);
#else
            System.Diagnostics.Debug.Assert(min < max);
#endif

            var range = max - min;
            var fromMin = (n - min) % range;

            if (fromMin >= 0)
            {
                return fromMin + min;
            }
            else
            {
                return max + fromMin;
            }
        }

        public static float InRange(float n, float max, float min = 0)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(min < max);
#else
            System.Diagnostics.Debug.Assert(min < max);
#endif

            var range = max - min;
            var fromMin = (n - min) % range;

            if (fromMin >= 0)
            {
                return fromMin + min;
            }
            else
            {
                return max + fromMin;
            }
        }

        /// <summary>
        /// 1次元配列に2次元配列のようにアクセスする
        /// </summary>
        public static int IndexD2ToD1(int x, int y, int height)
        {
            return x + y * height;
        }
    }
}
