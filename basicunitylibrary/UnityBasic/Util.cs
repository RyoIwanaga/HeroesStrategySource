using UnityEngine;
using System.Collections;

namespace UnityBasic
{
    public static class Util
    {
        public static Vector2 Vec3XZToVec2(Vector3 v3)
        {
            return new Vector2(v3.x, v3.z);
        }

        public static Vector3 Vec2ToVec3XZ(Vector2 v2, float y = 0f)
        {
            return new Vector3(v2.x, y, v2.y);
        }

        public static Vector3 Vec3AddVec2XZ(Vector3 b, Vector2 v2)
        {
            return new Vector3(b.x + v2.x, b.y, b.z + v2.y);
        }

        public static Vector3 Vec3MultiScalar(Vector3 v3, float s)
        {
            return new Vector3(v3.x * s, v3.y * s, v3.z * s);
        }

        /// <summary>
        /// For UI position
        /// </summary>
        public static Vector3 Vec3XYOnly(Vector3 vec3)
        {
            return new Vector3(vec3.x, vec3.y, 0f);
        }

        public static Color CreateInvisibleColor(Color c)
        {
            return new Color(c.r, c.g, c.b, 0f);
        }
    }
}
