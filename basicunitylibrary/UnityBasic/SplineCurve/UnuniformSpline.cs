using UnityEngine;
using System.Collections;

namespace UnityBehaviour.SplineCurve
{
    /// <summary>
    /// http://marupeke296.com/DXG_No34_UnuniformSprine.html
    /// </summary>
    public static class UnuniformSprine
    {
        public delegate Vector3 InterpolateFunction(float t, Vector3 p1 , Vector3 p2, Vector3 v1, Vector3 v2);

        public static Vector3 Interpolate(float t, Vector3 p1 , Vector3 p2, Vector3 v1, Vector3 v2)
        {
            var vecRowT = new float[]
            {
                t * t * t,
                t * t,
                t,
                1
            };

            var matH = new Matrix4x4();
            matH.m00 =  2; matH.m01 = -2; matH.m02 =  1; matH.m03 = 1;
            matH.m10 = -3; matH.m11 =  3; matH.m12 = -2; matH.m13 = -1;
            matH.m20 =  0; matH.m21 =  0; matH.m22 =  1; matH.m23 = 0;
            matH.m30 =  1; matH.m31 =  0; matH.m32 =  0; matH.m33 = 0;

            var result = new float[4];

            for (int x = 0; x < 4; x++)
            {
                float s = 0f;

                for (int y = 0; y < 4; y++)
                {
                    s += vecRowT[y] * matH[y, x];
                }

                result[x] = s;
            }

            Vector3 sum = UnityBasic.Util.Vec3MultiScalar(p1, result[0]);
            sum += UnityBasic.Util.Vec3MultiScalar(p2, result[1]);
            sum += UnityBasic.Util.Vec3MultiScalar(v1, result[2]);
            sum += UnityBasic.Util.Vec3MultiScalar(v2, result[3]);

            return sum;
        }
    }
}
