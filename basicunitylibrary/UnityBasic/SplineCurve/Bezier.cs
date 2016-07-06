using UnityEngine;

namespace UnityBehaviour.SplineCurve
{
    public static class Bezier
    {
        /// <summary>
        /// 1 control point version
        /// 
        /// CP = ControlPoint
        /// </summary>
        /// <param name="n1">start point</param>
        /// <param name="n2">first control point</param>
        /// <param name="n3">end point</param>
        /// <param name="t"></param>
        /// <returns></returns>
        static float Culculate(float n1, float n2, float n3, float t)
        {
            t = Mathf.Min(1f, t);
            t = Mathf.Max(0f, t);

            return Mathf.Pow(1f - t, 2) * n1
                + (1f - t) * 2 * t * n2
                + Mathf.Pow(t, 2) * n3;
        }

        /// <summary>
        /// 1 control point version
        /// 
        /// CP = ControlPoint
        /// </summary>
        /// <param name="v1">start point</param>
        /// <param name="v2">first control point</param>
        /// <param name="v3">end point</param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector2 Interpolate(Vector2 v1, Vector2 v2, Vector2 v3, float t)
        {
            return new Vector2(
                Culculate(v1.x, v2.x, v3.x, t),
                Culculate(v1.y, v2.y, v3.y, t));
        }

        /// <summary>
        /// 1 control point version
        /// 
        /// CP = ControlPoint
        /// </summary>
        /// <param name="v1">start point</param>
        /// <param name="v2">first control point</param>
        /// <param name="v3">end point</param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 Interpolate(Vector3 v1, Vector3 v2, Vector3 v3, float t)
        {
            return new Vector3(
                Culculate(v1.x, v2.x, v3.x, t),
                Culculate(v1.y, v2.y, v3.y, t),
                Culculate(v1.z, v2.z, v3.z, t));
        }
    }
}
